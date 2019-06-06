using System;
using Microsoft.Extensions.DependencyInjection;
using Stocker.Crawler.Tasks.Implementation;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 提供 <see cref="ICrawlerTaskManager"/> 的工厂对象。
    /// </summary>
    public interface ICrawlerTaskManagerFactory
    {
        /// <summary>
        /// 创建一个新的 <see cref="ICrawlerTaskManager"/> 实现。
        /// </summary>
        /// <returns></returns>
        ICrawlerTaskManager Create();
    }
}

namespace Stocker.Crawler.Tasks.DependencyInjection
{
    /// <summary>
    /// 为 <see cref="ICrawlerTaskManagerFactory"/> 提供依赖注入过程。
    /// </summary>
    public static class CrawlerTaskManagerFactoryExtensions
    {
        /// <summary>
        /// 将 <see cref="ICrawlerTaskManagerFactory"/> 的默认实现添加到服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/>为null</exception>
        public static IServiceCollection AddDefaultCrawlerTaskManagerFactory(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            return services.AddSingleton<ICrawlerTaskManagerFactory, DefaultCrawlerTaskManagerFactory>();
        }
    }
}
