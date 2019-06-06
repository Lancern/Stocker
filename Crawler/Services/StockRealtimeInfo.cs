using Newtonsoft.Json;

namespace Stocker.Crawler.Services
{
    /// <summary>
    /// 封装股票实时信息。
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
        /// 获取或设置总市值。
        /// </summary>
        [JsonProperty("mktcap")]
        public double MarketCapacity { get; set; }
        
        /// <summary>
        /// 获取或设置当前价格。
        /// </summary>
        [JsonProperty("trade")]
        public double CurrentPrice { get; set; }
        
        /// <summary>
        /// 获取或设置开盘价。
        /// </summary>
        [JsonProperty("open")]
        public double OpenPrice { get; set; }
        
        /// <summary>
        /// 获取或设置上一交易日收盘价。
        /// </summary>
        [JsonProperty("settlement")]
        public double SettlementPrice { get; set; }
        
        /// <summary>
        /// 获取或设置最高价。
        /// </summary>
        [JsonProperty("high")]
        public double HighestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置最低价。
        /// </summary>
        [JsonProperty("low")]
        public double LowestPrice { get; set; }
        
        /// <summary>
        /// 获取或设置股价的相对涨跌幅度。
        /// </summary>
        [JsonProperty("")]
        public double PriceRelativeChange { get; set; }
        
        /// <summary>
        /// 获取或设置换手率。
        /// </summary>
        [JsonProperty("turnoverratio")]
        public double TurnoverRatio { get; set; }
        
        /// <summary>
        /// 获取或设置成交量。
        /// </summary>
        [JsonProperty("volume")]
        public long Volume { get; set; }
        
        /// <summary>
        /// 获取或设置成交额。
        /// </summary>
        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}
