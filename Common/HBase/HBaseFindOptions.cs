namespace Stocker.HBase
{
    /// <summary>
    /// 封装查找数据行所需的信息。
    /// </summary>
    public sealed class HBaseFindOptions
    {
        /// <summary>
        /// 初始化 <see cref="HBaseFindOptions"/> 类的新实例。
        /// </summary>
        public HBaseFindOptions()
        {
            Column = null;
            Timestamp = null;
            NumberOfVersions = null;
        }
        
        public HBaseColumn Column { get; set; }
        
        /// <summary>
        /// 获取或设置期望的时间戳。
        /// </summary>
        public long? Timestamp { get; set; }
        
        /// <summary>
        /// 获取或设置在结果集中包含的版本数量。
        /// </summary>
        public int? NumberOfVersions { get; set; }
    }
}
