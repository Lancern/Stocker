using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stocker.Crawler.Services.Implementation;

namespace Stocker.Crawler.Services
{
    /// <summary>
    /// 提供到预测模型的消息通知。
    /// </summary>
    public interface IPredictorNotifier
    {
        /// <summary>
        /// 通知预测模型执行预测。
        /// </summary>
        /// <returns></returns>
        Task Notify();
    }
}

namespace Stocker.Crawler.Services.DependencyInjection
{
    /// <summary>
    /// 为 <see cref="IPredictorNotifier"/> 提供依赖注入过程。
    /// </summary>
    public static class PredictorNotifierExtensions
    {
        /// <summary>
        /// 将 <see cref="IPredictorNotifier"/> 的默认实现添加到服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="predictorHost">预测节点所在的主机。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="services"/>为null
        ///     或
        ///     <paramref name="predictorHost"/>为null
        /// </exception>
        public static IServiceCollection AddDefaultPredictorNotifier(this IServiceCollection services,
                                                                     string predictorHost)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (predictorHost == null)
                throw new ArgumentNullException(nameof(predictorHost));

            return services.AddSingleton<IPredictorNotifier, DefaultPredictorNotifier>(
                servicesProvider => new DefaultPredictorNotifier(
                    predictorHost,
                    servicesProvider.GetService<IHttpClientFactory>(),
                    servicesProvider.GetService<ILogger<DefaultPredictorNotifier>>()));
        }

        /// <summary>
        /// 将 <see cref="IPredictorNotifier"/> 的空实现添加到服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="services"/>为null
        /// </exception>
        public static IServiceCollection AddEmptyPredictorNotifier(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddSingleton<IPredictorNotifier, EmptyPredictorNotifier>();
        }
    }
}
