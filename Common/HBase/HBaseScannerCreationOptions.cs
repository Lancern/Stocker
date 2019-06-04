using System;
using System.Collections.Generic;

namespace Stocker.HBase
{
    /// <summary>
    /// 封装 HBase Scanner 的创建信息。
    /// </summary>
    public sealed class HBaseScannerCreationOptions
    {
        /// <summary>
        /// 初始化 <see cref="HBaseScannerCreationOptions"/> 类的新实例。
        /// </summary>
        public HBaseScannerCreationOptions()
        {
            Columns = null;
            StartRowKey = null;
            EndRowKey = null;
            Batch = null;
            StartTime = null;
            EndTime = null;
        }
        
        /// <summary>
        /// 获取或设置列。
        /// </summary>
        internal List<HBaseColumn> Columns { get; set; }
        
        /// <summary>
        /// 获取或设置 Scanner 的开始行键。
        /// </summary>
        public string StartRowKey { get; set; }
        
        /// <summary>
        /// 获取或设置 Scanner 的结束行键。
        /// </summary>
        public string EndRowKey { get; set; }
        
        /// <summary>
        /// 获取或设置 Scanner 一次性返回的数据行数量。
        /// </summary>
        public int? Batch { get; set; }
        
        /// <summary>
        /// 获取或设置起始时间戳。
        /// </summary>
        public int? StartTime { get; set; }
        
        /// <summary>
        /// 获取或设置结束时间戳。
        /// </summary>
        public int? EndTime { get; set; }

        /// <summary>
        /// 向 Scanner 添加列。
        /// </summary>
        /// <param name="column">要添加的列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="column"/>为null</exception>
        public void AddColumn(HBaseColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            
            if (Columns == null)
            {
                Columns = new List<HBaseColumn>();
            }
            Columns.Add(column);
        }
    }
}
