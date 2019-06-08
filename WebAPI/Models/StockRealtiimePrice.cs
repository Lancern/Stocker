using System;
using System.Linq;
using Newtonsoft.Json;
using Stocker.HBase;

namespace Stocker.WebAPI.Models
{
    /// <summary>
    /// 封装股票实时价格数据。
    /// </summary>
    public sealed class StockRealtiimePrice
    {
        /// <summary>
        /// 获取或设置实时价格数据的时间戳。
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// 获取或设置实时价格数据。
        /// </summary>
        [JsonProperty("price")]
        public PredictedNumber Price { get; set; }
    }
}
