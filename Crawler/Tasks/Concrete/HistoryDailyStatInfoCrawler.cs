using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stocker.Crawler.Services;
using Stocker.HBase;

namespace Stocker.Crawler.Tasks.Concrete
{
    [CrawlerTask(1)]
    public sealed class HistoryDailyStatInfoCrawler : ExclusiveStockCrawlerTaskBase
    {
        private static volatile bool _hasExecuted = false;

        private ILogger<HistoryDailyStatInfoCrawler> _logger;
        
        public HistoryDailyStatInfoCrawler(IStockInfoProvider stockInfoProvider,
                                           IHBaseClientFactory hBaseClientFactory,
                                           ILogger<HistoryDailyStatInfoCrawler> logger)
            : base(stockInfoProvider, hBaseClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task RunExclusive()
        {
            if (_hasExecuted)
            {
                return;
            }

            _hasExecuted = true;
            
            // 获取股票列表
            var stocksList = await StockInfoProvider.GetRealtimeStocksList();
            
            _logger.LogInformation("股票列表获取成功。一共获取到 {0} 支股票的信息。", stocksList.Count);

            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddYears(-1);

            var dict = new Dictionary<string, List<StockDailyStatisticsInfo>>();
            foreach (var stockCode in stocksList.Select(stock => stock.Code))
            {
                var stockDailyInfos = await StockInfoProvider.GetDailyStatisticsInfo(stockCode, startDate, endDate);
                dict.Add(stockCode, stockDailyInfos);
            }
            
            _logger.LogInformation("股票历史记录日统计信息爬取完成。序列化至本地文件 HistoryData.json。");
            
            // 打开本地文件以进行写入
            var json = JsonConvert.SerializeObject(dict);
            await File.WriteAllTextAsync("HistoryData.json", json, Encoding.UTF8);
            
            _logger.LogInformation("股票历史记录日统计数据序列化完成。文件为 HistoryData.json。");
        }
    }
}
