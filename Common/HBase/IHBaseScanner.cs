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
        /// 获取 Scanner 的 ID。
        /// </summary>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出。</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        string Id { get; }
        
        /// <summary>
        /// 获取 Scanner 是否已经读取完毕所有满足条件的数据行。
        /// </summary>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出。</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        bool HasExhausted { get; }

        /// <summary>
        /// 读取下一个 Batch 中的所有数据行。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出。</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<List<HBaseRow>> ReadNextBatch();
    }
}
