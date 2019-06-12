using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Stocker.HBase;

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

        public static StockRealtimeInfo FromHBaseRowCellCollection(
            HBaseRowCellCollection realtimeCells)
        {
            if (realtimeCells == null)
                throw new ArgumentNullException(nameof(realtimeCells));

            var stock = new StockRealtimeInfo
            {
                Data = new List<StockRealtiimePrice>()
            };

            var realtimePrice = realtimeCells.Get("price", "price").ToArray();
            foreach (var price in realtimePrice)
            {
                stock.Data.Add(new StockRealtiimePrice
                {
                    Timestamp = new DateTime(price.Timestamp),
                    Price = new PredictedNumber { ActualValue = double.Parse(Encoding.UTF8.GetString(price.Data)) }
                });
            }

            return stock;
        }
    }
}
