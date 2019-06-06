namespace Stocker.HBase
{
    /// <summary>
    /// 封装 HBase 中一个单元格的数据信息。
    /// </summary>
    public sealed class HBaseCell
    {
        /// <summary>
        /// 获取或设置单元格的列。
        /// </summary>
        public HBaseColumn Column { get; set; }
        
        /// <summary>
        /// 获取或设置单元格中的数据。
        /// </summary>
        public byte[] Data { get; set; }
        
        /// <summary>
        /// 获取或设置单元格的时间戳。
        /// </summary>
        public long Timestamp { get; set; }
    }
}
