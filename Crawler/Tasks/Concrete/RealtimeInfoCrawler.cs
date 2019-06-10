using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stocker.Crawler.Services;
using Stocker.HBase;

namespace Stocker.Crawler.Tasks.Concrete
{
    /// <summary>
    /// 提供实时数据爬虫的逻辑。
    /// </summary>
    [CrawlerTask(3)]
    public sealed class RealtimeInfoCrawler : ExclusiveStockCrawlerTaskBase
    {
        private const string RealtimeDataHBaseTableName = "StockInfoRealTime";

        private readonly IPredictorNotifier _predictorNotifier;
        
        /// <summary>
        /// 初始化 <see cref="RealtimeInfoCrawler"/> 类的新实例。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="stockInfoProvider"/>为null
        ///     或
        ///     <paramref name="hBaseClientFactory"/>为null
        ///     或
        ///     <paramref name="predictorNotifier"/>为null
        /// </exception>
        public RealtimeInfoCrawler(IStockInfoProvider stockInfoProvider, 
                                   IHBaseClientFactory hBaseClientFactory,
                                   IPredictorNotifier predictorNotifier)
            : base(stockInfoProvider, hBaseClientFactory)
        {
            _predictorNotifier = predictorNotifier ?? throw new ArgumentNullException(nameof(predictorNotifier));
        }

        /// <summary>
        /// 从给定的股票实时数据对象创建 HBase 行。
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        private static HBaseRow GetRow(StockRealtimeInfo stockInfo, DateTime timestamp)
        {
            var row = new HBaseRow { Key = stockInfo.Code };
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("name", "name"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.Name)
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("price", "price"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.CurrentPrice.ToString("G"))
            });
            
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("priceChange", "priceChange"),
                Timestamp = timestamp.Ticks,
                Data = Encoding.UTF8.GetBytes(stockInfo.PriceRelativeChange.ToString("G"))
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
            if (ts.DayOfWeek == DayOfWeek.Saturday || ts.DayOfWeek == DayOfWeek.Sunday)
            {
                // 周六周日不开市
                return;
            }

            if (ts.TimeOfDay < new TimeSpan(9, 30, 0) || 
                ts.TimeOfDay > new TimeSpan(15, 0, 0))
            {
                // 股市未开市
                return;
            }
            
            var stocksList = await StockInfoProvider.GetRealtimeStocksList();

            // 将新获取的数据加入到 HBase 中
            var hbaseRows = stocksList.Select(item => GetRow(item, ts));
            await HBaseClientFactory.Create().Add(RealtimeDataHBaseTableName, hbaseRows);
            
            // 通知预测节点
            await _predictorNotifier.Notify();
        }
    }
}
