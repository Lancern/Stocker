using System;

namespace Stocker.HBase
{
    /// <summary>
    /// 为 <see cref="IHBaseClient"/> 提供工厂类抽象。
    /// </summary>
    public interface IHBaseClientFactory
    {
        /// <summary>
        /// 创建访问指定 URL 处的 HBase 实例的 <see cref="IHBaseClient"/> 对象。
        /// </summary>
        /// <param name="hbaseHostUrl">HBase 实例所在的 URL。</param>
        /// <param name="port">HBase 实例的监听端口。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="hbaseHostUrl"/>为null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="port"/>小于0 或 大于65535
        /// </exception>
        IHBaseClient Create(string hbaseHostUrl, int port);
    }
}
