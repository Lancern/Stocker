namespace Stocker.HBase
{
    /// <summary>
    /// 封装 HBase 的行数据。
    /// </summary>
    public sealed class HBaseRow
    {
        /// <summary>
        /// 初始化 <see cref="HBaseRow"/> 的新实例。
        /// </summary>
        public HBaseRow()
        {
            Key = null;
            Cells = new HBaseRowCellCollection();
        }
        
        /// <summary>
        /// 获取或设置当前行的键。
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 获取当前的行所包含的列集合。
        /// </summary>
        public HBaseRowCellCollection Cells { get; }
    }
}
