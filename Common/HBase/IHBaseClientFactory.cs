using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stocker.HBase.Implementations;

namespace Stocker.HBase
{
    /// <summary>
    /// 为 <see cref="IHBaseClient"/> 提供工厂类抽象。
    /// </summary>
    public interface IHBaseClientFactory
    {
        /// <summary>
        /// 根据 <see cref="IConfiguration"/> 中的配置创建到 HBase 实例的 <see cref="IHBaseClient"/> 对象。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 未能在 <see cref="IConfiguration"/> 中找到对应的配置项。
        /// </exception>
        IHBaseClient Create();
        
        /// <summary>
        /// 创建访问指定 URL 处的 HBase 实例的 <see cref="IHBaseClient"/> 对象。
        /// </summary>
        /// <param name="address">HBase 实例所在的主机地址。</param>
        /// <param name="port">HBase 实例的监听端口。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="address"/>为null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="port"/>小于0 或 大于65535
        /// </exception>
        IHBaseClient Create(string address, int port);
    }
}

namespace Stocker.HBase.DependencyInjection
{
    /// <summary>
    /// 为 <see cref="IHBaseClientFactory"/> 提供依赖注入过程。
    /// </summary>
    public static class HBaseClientFactoryExtensions
    {
        /// <summary>
        /// 将 <see cref="IHBaseClientFactory"/> 的默认实现添加到服务容器中。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/>为null
        /// </exception>
        public static IServiceCollection AddHBaseClientFactory(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            return services.AddTransient<IHBaseClientFactory, DefaultHBaseClientFactory>();
        }
    }
}
