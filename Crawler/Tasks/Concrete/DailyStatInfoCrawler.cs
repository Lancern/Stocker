using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private const string HBaseTableName = "stocks";

        private readonly IPredictorNotifier _predictorNotifier;
        
        /// <summary>
        /// 初始化 <see cref="DailyStatInfoCrawler"/> 类的新实例。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="stockInfoProvider"/>为null
        ///     或
        ///     <paramref name="hBaseClientFactory"/>为null
        ///     或
        ///     <paramref name="predictorNotifier"/>为null
        /// </exception>
        public DailyStatInfoCrawler(IStockInfoProvider stockInfoProvider, 
                                    IHBaseClientFactory hBaseClientFactory,
                                    IPredictorNotifier predictorNotifier)
            : base(stockInfoProvider, hBaseClientFactory)
        {
            _predictorNotifier = predictorNotifier ?? throw new ArgumentNullException(nameof(predictorNotifier));
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

        /// <summary>
        /// 从给定的 <see cref="ProducerConsumerQueue{String}"/> 中获取要爬取的股票代码并爬取日统计信息。
        /// </summary>
        /// <param name="stockCodeQueue"></param>
        /// <param name="timestamp">爬取的数据所使用的时间戳</param>
        /// <returns></returns>
        private async Task ConsumeAndCrawl(ProducerConsumerQueue<string> stockCodeQueue, DateTime timestamp)
        {
            var date = DateTime.Now.Date;
            var stockInfosList = new List<StockDailyStatisticsInfo>();
            while (true)
            {
                string stockCode;
                try
                {
                    stockCode = stockCodeQueue.Dequeue();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                var stockStat = await StockInfoProvider.GetDailyStatisticsInfo(stockCode, date, date);
                if (stockStat != null)
                {
                    stockInfosList.Add(stockStat);
                }
            }

            var rows = stockInfosList.Select(item => GetRow(item, timestamp));
            await HBaseClientFactory.Create().Add(HBaseTableName, rows);
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
            var stocksCodeList = new List<string>();
            var hbaseClient = HBaseClientFactory.Create();
            var scannerCreationOptions = new HBaseScannerCreationOptions { Batch = 1000 };
            using (var scanner = await hbaseClient.OpenScanner(HBaseTableName, scannerCreationOptions))
            {
                while (await scanner.ReadNextBatch())
                {
                    stocksCodeList.AddRange(scanner.CurrentBatch.Select(row => row.Key));
                }
            }
            
            // API 调用限制：500毫秒间隔，最大并发请求数 = 3
            var workQueue = new ProducerConsumerQueue<string>(3);
            var consumer = Task.Run(() => ConsumeAndCrawl(workQueue, ts));
            foreach (var stockCode in stocksCodeList)
            {
                workQueue.Enqueue(stockCode);
                await Task.Delay(500);
            }

            await consumer;
            
            // 通知预测节点
            await _predictorNotifier.Notify();
        }
    }
}
