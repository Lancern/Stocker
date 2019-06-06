using System;
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
        /// <summary>
        /// 初始化 <see cref="RealtimeInfoCrawler"/> 类的新实例。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="stockInfoProvider"/>为null
        ///     或
        ///     <paramref name="hBaseClientFactory"/>为null
        /// </exception>
        public RealtimeInfoCrawler(IStockInfoProvider stockInfoProvider, IHBaseClientFactory hBaseClientFactory)
            : base(stockInfoProvider, hBaseClientFactory)
        {
        }

        protected override async Task RunExclusive()
        {
            throw new NotImplementedException();
        }
    }
}
