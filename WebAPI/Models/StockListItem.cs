using System;
using Newtonsoft.Json;

namespace Stocker.WebAPI.Models
{
    /// <summary>
    /// 为股票列表中的数据项提供数据模型。
    /// </summary>
    public sealed class StockListItem
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
        /// 获取或设置股票实时价格。
        /// </summary>
        [JsonProperty("price")]
        public double Price { get; set; }
        
        /// <summary>
        /// 获取或设置股票价格相对变化幅度的百分比表示。
        /// </summary>
        [JsonProperty("priceRelativeChangePercent")]
        public double PriceRelativeChangePercent { get; set; }
        
        /// <summary>
        /// 获取或设置数据更新时间。
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime UpdateTime { get; set; }
    }
}
