using System;
using Newtonsoft.Json;

namespace Stocker.WebAPI.Models
{
    /// <summary>
    /// 为股票日统计数据提供数据模型。
    /// </summary>
    public sealed class StockDailyInfo
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
        /// 获取或设置股票日统计数据的日期。
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        
        /// <summary>
        /// 获取或设置开盘价格。
        /// </summary>
        [JsonProperty("openPrice")]
        public PredictedNumber OpenPrice { get; set; }
        
        /// <summary>
        /// 获取或设置收盘价格。
        /// </summary>
        [JsonProperty("settlementPrice")]
        public PredictedNumber SettlementPrice { get; set; }
        
        /// <summary>
        /// 获取或设置最低价格。
        /// </summary>
        [JsonProperty("LowestPrice")]
        public PredictedNumber LowestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置最高价格。
        /// </summary>
        [JsonProperty("highestPrice")]
        public PredictedNumber HighestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置总成交量。
        /// </summary>
        [JsonProperty("totalDeals")]
        public PredictedNumber TotalDeals { get; set; }
        
        /// <summary>
        /// 获取或设置总成交额。
        /// </summary>
        [JsonProperty("amount")]
        public PredictedNumber Amount { get; set; }
    }
}
