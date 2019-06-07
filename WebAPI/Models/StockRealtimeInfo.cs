using System.Collections.Generic;
using Newtonsoft.Json;

namespace Stocker.WebAPI.Models
{
    /// <summary>
    /// 为股票实时数据提供数据模型。
    /// </summary>
    public sealed class StockRealtimeInfo
    {
        /// <summary>
        /// 获取或设置股票代码。
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        
        /// <summary>
        /// 获取或设置股票名称。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置股票实时数据列表。
        /// </summary>
        [JsonProperty("data")]
        public List<StockRealtiimePrice> Data { get; set; }
    }
}
