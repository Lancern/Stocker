using System;
using Stocker.Crawler.Services;
using Stocker.HBase;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 为股票数据 Crawler Task 提供抽象基类。
    /// </summary>
    public abstract class ExclusiveStockCrawlerTaskBase : ExclusiveCrawlerTask
    {
        /// <summary>
        /// 初始化 <see cref="ExclusiveStockCrawlerTaskBase"/> 类的新实例。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="stockInfoProvider"/>为null
        ///     或
        ///     <paramref name="hBaseClientFactory"/>为null
        /// </exception>
        protected ExclusiveStockCrawlerTaskBase(IStockInfoProvider stockInfoProvider,
                                                IHBaseClientFactory hBaseClientFactory)
        {
            StockInfoProvider = stockInfoProvider ?? throw new ArgumentNullException(nameof(stockInfoProvider));
            HBaseClientFactory = hBaseClientFactory ?? throw new ArgumentNullException(nameof(hBaseClientFactory));
        }
        
        /// <summary>
        /// 获取股票信息源。
        /// </summary>
        protected IStockInfoProvider StockInfoProvider { get; }
        
        /// <summary>
        /// 获取 HBase Client 对象的工厂对象。
        /// </summary>
        protected IHBaseClientFactory HBaseClientFactory { get; }

        /// <summary>
        /// 使用 <see cref="HBaseClientFactory"/> 创建 <see cref="IHBaseClient"/> 实例对象。
        /// </summary>
        /// <returns></returns>
        protected IHBaseClient CreateHBaseClient()
        {
            return HBaseClientFactory.Create();
        }
    }
}
