using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocker.HBase
{
    /// <summary>
    /// 提供从 REST API 访问 HBase 实例的方法。
    /// </summary>
    public interface IHBaseClient : IDisposable
    {
        /// <summary>
        /// 向 HBase 中添加数据行。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="rows">要添加的数据行。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="tableName"/>为null
        ///     或
        ///     <paramref name="rows"/>为null
        /// </exception>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task Add(string tableName, IEnumerable<HBaseRow> rows);

        /// <summary>
        /// 从 HBase 中查找数据行。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="rowKey">要查找的行键。</param>
        /// <param name="options">可选参数。额外的查询参数。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="tableName"/>为null
        ///     或
        ///     <paramref name="rowKey"/>为null
        /// </exception>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<HBaseRow> Find(string tableName, string rowKey, HBaseFindOptions options = null);

        /// <summary>
        /// 打开一个 Scanner 以便从 HBase 中批量读取数据。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="options">可选参数。Scanner 的创建选项。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="tableName"/>为null
        /// </exception>
        /// <exception cref="HBaseException">当访问 HBase REST API 发生错误时抛出</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<IHBaseScanner> OpenScanner(string tableName, HBaseScannerCreationOptions options = null);
    }
}
