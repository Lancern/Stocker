namespace Stocker.HBase
{
    /// <summary>
    /// 封装 HBase 中一个单元格的数据信息。
    /// </summary>
    public sealed class HBaseCell
    {
        /// <summary>
        /// 获取或设置单元格所处的列族的名称。
        /// </summary>
        public string ColumnFamilyName { get; set; }
        
        /// <summary>
        /// 获取或设置单元格所处的列名称。
        /// </summary>
        public string ColumnName { get; set; }
        
        /// <summary>
        /// 获取或设置单元格中的数据。
        /// </summary>
        public byte[] Data { get; set; }
        
        /// <summary>
        /// 获取或设置单元格的时间戳。
        /// </summary>
        public int Timestamp { get; set; }

        /// <summary>
        /// 获取当前单元格的列标识符。
        /// </summary>
        /// <returns>当前单元格的列标识符</returns>
        public string GetColumnIdentifier()
        {
            return $"{ColumnFamilyName}:{ColumnName}";
        }
    }
}
