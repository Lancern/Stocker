using System;
using Newtonsoft.Json;

namespace Stocker.Crawler.Services
{
    /// <summary>
    /// 封装股票信息。
    /// </summary>
    public sealed class StockInfo
    {
        /// <summary>
        /// 获取或设置股票代码。
        /// </summary>
        [JsonProperty("symbol")]
        public string Code { get; set; }
        
        /// <summary>
        /// 获取或设置股票名称。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置股票的最新价格。
        /// </summary>
        [JsonProperty("trade")]
        public double LatestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置股票价格的绝对增长量。
        /// </summary>
        [JsonProperty("pricechange")]
        public double PriceAbsoluteChange { get; set; }
        
        /// <summary>
        /// 获取或设置股票价格的相对增长量。
        /// </summary>
        [JsonProperty("changepercent")]
        public double PriceRelativeChange { get; set; }
        
        /// <summary>
        /// 获取或设置买入价格。
        /// </summary>
        [JsonProperty("buy")]
        public double BuyPrice { get; set; }
        
        /// <summary>
        /// 获取或设置卖出价格。
        /// </summary>
        [JsonProperty("sell")]
        public double SellPrice { get; set; }
        
        /// <summary>
        /// 获取或设置上一交易日收盘价格。
        /// </summary>
        [JsonProperty("settlement")]
        public double SettlementPrice { get; set; }
        
        /// <summary>
        /// 获取或设置当前交易日开盘价格。
        /// </summary>
        [JsonProperty("open")]
        public double OpenPrice { get; set; }
        
        /// <summary>
        /// 获取或设置股票的最高价格。
        /// </summary>
        [JsonProperty("high")]
        public double HighestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置股票的最低价格。
        /// </summary>
        [JsonProperty("low")]
        public double LowestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置股票成交量。
        /// </summary>
        [JsonProperty("volume")]
        public long TradeVolume { get; set; }
        
        /// <summary>
        /// 获取或设置股票成交额。
        /// </summary>
        [JsonProperty("amount")]
        public long TradeAmount { get; set; }
        
        /// <summary>
        /// 获取或设置数据更新时间戳。
        /// </summary>
        [JsonProperty("ticktime")]
        public DateTime Timestamp { get; set; }
    }
}
