using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Stocker.Crawler.Services;
using Stocker.Crawler.Services.Implementation;

namespace Stocker.Crawler.Services
{
    /// <summary>
    /// 为应用程序提供业务逻辑入口。
    /// </summary>
    public interface IApplicationRunner
    {
        /// <summary>
        /// 以异步方式启动应用程序的业务逻辑。
        /// </summary>
        Task Run();
    }
}

namespace Stocker.Crawler.Services.DependencyInjection
{
    /// <summary>
    /// 为 <see cref="IApplicationRunner"/> 提供依赖注入逻辑。
    /// </summary>
    public static class ApplicationRunnerExtensions
    {
        /// <summary>
        /// 将默认的 <see cref="IApplicationRunner"/> 实现添加到给定的服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/>为null
        /// </exception>
        public static IServiceCollection AddApplicationRunner(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddSingleton<IApplicationRunner, DefaultApplicationRunner>();
        }
    }
}
