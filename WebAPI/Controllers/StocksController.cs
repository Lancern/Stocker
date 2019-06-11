using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Stocker.HBase;
using Stocker.WebAPI.Models;

namespace Stocker.WebAPI.Controllers
{
    /// <summary>
    /// 为路径 /stocks/* 提供控制器。
    /// </summary>
    [ApiController]
    [Route("stocks")]
    public class StocksController : ControllerBase
    {
        private const string DailyDataTableName = "StockInfoOfDay";
        private const string RealtimeDataTableName = "StockInfoRealTime";
        private const string PredictionDataTableName = "StockInfoOfPrediction";
        
        private readonly IHBaseClientFactory _hbaseClientFactory;
        private readonly ILogger<StocksController> _logger;

        /// <summary>
        /// 初始化 <see cref="StocksController"/> 类的新实例。
        /// </summary>
        /// <param name="hbaseClientFactory">用于创建 <see cref="IHBaseClient"/> 对象的工厂对象。</param>
        /// <param name="logger">日志支撑件。</param>
        public StocksController(IHBaseClientFactory hbaseClientFactory, ILogger<StocksController> logger)
        {
            _hbaseClientFactory = hbaseClientFactory ?? throw new ArgumentNullException(nameof(hbaseClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: /stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockListItem>>> Get(
            [FromQuery] string search,
            [FromQuery] int? page,
            [FromQuery] int? itemsPerPage)
        {
            if (((page == null ? 1 : 0) ^ (itemsPerPage == null ? 1 : 0)) != 0)
            {
                // page 或 itemsPerPage 没有同时提供
                return BadRequest();
            }

            var hbaseClient = _hbaseClientFactory.Create();
            var scannerCreationOptions = new HBaseScannerCreationOptions
            {
                Batch = 1000,
                MaxVersions = 1
            };

            // 获取所有的股票信息的列表
            IEnumerable<StockListItem> stocksList = Array.Empty<StockListItem>();
            using (var scanner = await hbaseClient.OpenScanner(DailyDataTableName, scannerCreationOptions))
            {
                while (await scanner.ReadNextBatch())
                {
                    stocksList = stocksList.Concat(scanner.CurrentBatch.Select(StockListItem.FromHBaseRow));
                }
            }

            // 在线程池线程中完成搜索和分页任务
            stocksList = await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(search))
                {
                    // 执行搜索
                    stocksList = stocksList.Where(
                        stock => stock.Code.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) != -1 ||
                                 stock.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) != -1);
                }

                if (page != null && itemsPerPage != null)
                {
                    stocksList = stocksList.Skip(page.Value * itemsPerPage.Value).Take(itemsPerPage.Value);
                }

                return stocksList;
            });
            
            return new ActionResult<IEnumerable<StockListItem>>(stocksList);
        }
        
        // GET: /stocks/{code}/realtime
        [HttpGet("{code}/realtime")]
        public async Task<ActionResult<StockRealtimeInfo>> GetStockRealtimeInfo(
            string code,
            [FromQuery] DateTime? date)
        {
            if (date == null)
            {
                date = DateTime.Now;
            }

            var hbaseClient = _hbaseClientFactory.Create();

            var dayRow = await hbaseClient.Find(DailyDataTableName, code);
            if (dayRow == null)
            {
                return NotFound();
            }

            // 获取股票名称
            var name = Encoding.UTF8.GetString(dayRow.Cells.Get("name", "name").ToArray()[0].Data);

            // 获取timestamp在当天的所有cell
            var dayCells = new HBaseRowCellCollection();
            foreach (var cell in dayRow.Cells)
            {
                if (new DateTime(cell.Timestamp).Date == date.Value.Date)
                {
                    dayCells.Add(cell);
                }
            }

            var predictRow = await hbaseClient.Find(PredictionDataTableName, code);

            var predictCells = new HBaseRowCellCollection();
            foreach (var cell in predictRow.Cells)
            {
                if (new DateTime(cell.Timestamp).Date == date.Value.Date)
                {
                    dayCells.Add(cell);
                }
            }

            // 合并真实值和预测值
            var realtimeInfo = StockRealtimeInfo.FromHBaseRowCellCollection(dayCells, predictCells);
            realtimeInfo.Code = code;
            realtimeInfo.Name = name;

            return new ActionResult<StockRealtimeInfo>(realtimeInfo);
        }

        // GET: /stocks/{code}/daily
        [HttpGet("{code}/daily")]
        public async Task<ActionResult<IEnumerable<StockDailyInfo>>> GetStockDailyInfo(
            string code,
            [FromQuery][BindRequired] DateTime startDate,
            [FromQuery][BindRequired] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest();

            var hbaseClient = _hbaseClientFactory.Create();

            var dayRow = await hbaseClient.Find(DailyDataTableName, code);
            if (dayRow == null)
            {
                return NotFound();
            }

            var name = Encoding.UTF8.GetString(dayRow.Cells.Get("name", "name").ToArray()[0].Data);

            var dayCells = new HBaseRowCellCollection();
            foreach (var cell in dayRow.Cells)
            {
                if (cell.Timestamp >= startDate.Ticks && cell.Timestamp <= endDate.Ticks)
                {
                    dayCells.Add(cell);
                }
            }

            var predictRow = await hbaseClient.Find(PredictionDataTableName, code);

            var predictCells = new HBaseRowCellCollection();
            foreach (var cell in predictRow.Cells)
            {
                if (cell.Timestamp >= startDate.Ticks && cell.Timestamp <= endDate.Ticks)
                {
                    dayCells.Add(cell);
                }
            }
            
            var dailyInfo = new List<StockDailyInfo>();
            foreach (var item in StockDailyInfo.FromHBaseRowCellCollection(dayCells, predictCells))
            {
                var info = item.Value;
                info.Code = code;
                info.Name = name;
                dailyInfo.Add(info);
            }

            return new ActionResult<IEnumerable<StockDailyInfo>>(dailyInfo);
        }
    }
}
