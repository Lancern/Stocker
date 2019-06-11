using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stocker.Crawler.Services;
using Stocker.Crawler.Utils;
using Stocker.HBase;

namespace Stocker.Crawler.Tasks.Concrete
{
    /// <summary>
    /// 提供日统计数据爬虫的逻辑。
    /// </summary>
    [CrawlerTask(60)]
    public sealed class DailyStatInfoCrawler : ExclusiveStockCrawlerTaskBase
    {
        private const string DailyDataHBaseTableName = "StockInfoOfDay";

        private readonly IPredictorNotifier _predictorNotifier;
        private readonly ILogger<DailyStatInfoCrawler> _logger;
        
        /// <summary>
        /// 初始化 <see cref="DailyStatInfoCrawler"/> 类的新实例。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="stockInfoProvider"/>为null
        ///     或
        ///     <paramref name="hBaseClientFactory"/>为null
        ///     或
        ///     <paramref name="predictorNotifier"/>为null
        ///     或
        ///     <paramref name="logger"/>为null
        /// </exception>
        public DailyStatInfoCrawler(IStockInfoProvider stockInfoProvider, 
                                    IHBaseClientFactory hBaseClientFactory,
                                    IPredictorNotifier predictorNotifier,
                                    ILogger<DailyStatInfoCrawler> logger)
            : base(stockInfoProvider, hBaseClientFactory)
        {
            _predictorNotifier = predictorNotifier ?? throw new ArgumentNullException(nameof(predictorNotifier));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取给定股票日统计数据所对应的 HBase 数据行。
        /// </summary>
        /// <param name="stockInfo">股票日统计数据</param>
        /// <param name="timestamp">获取数据时的时间戳</param>
        /// <returns></returns>
        private static HBaseRow GetRow(StockDailyStatisticsInfo stockInfo, DateTime timestamp)
        {
            var row = new HBaseRow { Key = stockInfo.Code };
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("open", "open"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.OpenPrice.ToString("G"))
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("close", "close"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.ClosePrice.ToString("G"))
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("highest", "highest"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.HighestPrice.ToString("G"))
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("lowest", "lowest"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.LowestPrice.ToString("G"))
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("total", "total"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.Volume.ToString("G"))
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("date", "date"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(timestamp.ToString("yyyy-M-d hh:mm:ss"))
            });

            return row;
        }

        /// <inheritdoc />
        protected override async Task RunExclusive()
        {
            var ts = DateTime.Now;
            if (ts.TimeOfDay < new TimeSpan(15, 0, 0) ||
                ts.TimeOfDay > new TimeSpan(16, 0, 0))
            {
                // 15:00 - 16:00 以外时间不执行
                return;
            }

            if (ts.DayOfWeek == DayOfWeek.Saturday || ts.DayOfWeek == DayOfWeek.Sunday)
            {
                // 周六周日不执行
                return;
            }
            
            // 获取所有的股票代码
            var stocksList = await StockInfoProvider.GetRealtimeStocksList();
            
            _logger.LogInformation("获取所有股票代码完毕。一共获取到 {0} 支股票代码。", stocksList.Count);
            
            // 爬取所有股票的日统计数据
            var stockInfosList = new List<StockDailyStatisticsInfo>();
            foreach (var stockCode in stocksList.Select(stock => stock.Code))
            {
                var stockStatList = await StockInfoProvider.GetDailyStatisticsInfo(stockCode, ts.Date, ts.Date);
                var stockStat = stockStatList.FirstOrDefault();
                if (stockStat != null)
                {
                    stockInfosList.Add(stockStat);
                }
            }
            
            // 将爬取到的日统计数据写入 HBase
            var rows = stockInfosList.Select(item => GetRow(item, ts));
            await HBaseClientFactory.Create().Add(DailyDataHBaseTableName, rows);
            
            _logger.LogTrace("准备通知预测节点数据更新");
            
            // 通知预测节点
            await _predictorNotifier.Notify();
        }
    }
}
