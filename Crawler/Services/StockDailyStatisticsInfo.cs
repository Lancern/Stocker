using System;
using Newtonsoft.Json;

namespace Stocker.Crawler.Services
{
    /// <summary>
    /// 封装股票的日交易数据信息。
    /// </summary>
    public sealed class StockDailyStatisticsInfo
    {
        /// <summary>
        /// 获取或设置股票代码。
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        
        /// <summary>
        /// 获取或设置开盘价。
        /// </summary>
        [JsonProperty("open")]
        public double OpenPrice { get; set; }
        
        /// <summary>
        /// 获取或设置收盘价。
        /// </summary>
        [JsonProperty("close")]
        public double ClosePrice { get; set; }
        
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
        /// 获取或设置数据日期。
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        
        /// <summary>
        /// 获取或设置成交量。
        /// </summary>
        [JsonProperty("volume")]
        public int Volume { get; set; }
    }
}
