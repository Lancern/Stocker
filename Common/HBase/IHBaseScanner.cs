using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocker.HBase
{
    /// <summary>
    /// 为 HBase Scanner 提供抽象。
    /// </summary>
    public interface IHBaseScanner : IDisposable
    {
        /// <summary>
        /// 获取上一次调用 <see cref="ReadNextBatch"/> 时从 HBase 实例中读取的 Batch 数据。
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        List<HBaseRow> CurrentBatch { get; }

        /// <summary>
        /// 读取下一个 Batch 中的所有数据行。
        /// </summary>
        /// <returns>是否读取了至少一个数据行。</returns>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出。</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<bool> ReadNextBatch();
    }
}
