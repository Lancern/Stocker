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
    /// 为股票数据源提供抽象。
    /// </summary>
    public interface IStockInfoProvider
    {
        /// <summary>
        /// 获取实时股票数据列表。
        /// </summary>
        /// <returns>当获取数据失败时，返回空列表。</returns>
        Task<List<StockRealtimeInfo>> GetRealtimeStocksList();

        /// <summary>
        /// 获取股票的日统计数据。
        /// </summary>
        /// <param name="code">股票代码</param>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>当获取数据失败时，返回空列表。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="code"/>为null</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="startDate"/>晚于<paramref name="endDate"/>
        /// </exception>
        Task<List<StockDailyStatisticsInfo>> GetDailyStatisticsInfo(string code, DateTime startDate, DateTime endDate);
    }
}

namespace Stocker.Crawler.Services.DependencyInjection
{
    /// <summary>
    /// 为 <see cref="IStockInfoProvider"/> 提供依赖注入过程。
    /// </summary>
    public static class StockInfoProviderExtensions
    {
        /// <summary>
        /// 将 <see cref="IStockInfoProvider"/> 的默认实现添加到服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="stockListAppId">用于访问股票实时数据的 App ID。</param>
        /// <param name="stockDailyStatAppId">用于访问股票日统计数据 API 的 App ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="services"/>为null
        ///     或
        ///     <paramref name="stockListAppId"/>为null
        /// </exception>
        public static IServiceCollection AddDefaultCrawler(this IServiceCollection services, 
                                                           string stockListAppId, string stockDailyStatAppId)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (stockListAppId == null)
                throw new ArgumentNullException(nameof(stockListAppId));

            return services.AddSingleton<IStockInfoProvider, DefaultStockInfoProvider>(
                serviceProvider => new DefaultStockInfoProvider(serviceProvider.GetService<ILogger<DefaultStockInfoProvider>>(),
                                                      serviceProvider.GetService<IHttpClientFactory>(),
                                                      stockListAppId, stockDailyStatAppId));
        }
    }
}
