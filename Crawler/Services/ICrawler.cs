using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stocker.Crawler.Services.Implementation;

namespace Stocker.Crawler.Services
{
    /// <summary>
    /// 为股票实时数据爬虫提供抽象。
    /// </summary>
    public interface ICrawler
    {
        /// <summary>
        /// 获取实时股票数据列表。
        /// </summary>
        /// <returns></returns>
        Task<List<StockInfo>> GetStocksList();
    }
}

namespace Stocker.Crawler.Services.DependencyInjection
{
    /// <summary>
    /// 为 <see cref="ICrawler"/> 提供依赖注入过程。
    /// </summary>
    public static class CrawlerExtensions
    {
        /// <summary>
        /// 将 <see cref="ICrawler"/> 的默认实现添加到服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appId">用于访问第三方服务的 App ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="services"/>为null
        ///     或
        ///     <paramref name="appId"/>为null
        /// </exception>
        public static IServiceCollection AddDefaultCrawler(this IServiceCollection services, string appId)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (appId == null)
                throw new ArgumentNullException(nameof(appId));

            return services.AddSingleton<ICrawler, DefaultCrawler>(
                serviceProvider => new DefaultCrawler(serviceProvider.GetService<ILogger<DefaultCrawler>>(),
                                                      serviceProvider.GetService<IHttpClientFactory>(),
                                                      appId));
        }
    }
}
