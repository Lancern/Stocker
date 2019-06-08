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
            HBaseRowCellCollection dayCells,
            HBaseRowCellCollection predictCells)
        {
            if (dayCells == null)
                throw new ArgumentNullException(nameof(dayCells));

            var stock = new StockRealtimeInfo
            {
                Data = new List<StockRealtiimePrice>()
            };

            var nameCells = dayCells.Get("name", "name").ToArray();
            if (nameCells.Length > 0)
            {
                stock.Name = Encoding.UTF8.GetString(nameCells[0].Data);
            }

            foreach (var value in PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "price"))
            {
                stock.Data.Add(new StockRealtiimePrice
                {
                    Timestamp = new DateTime(value.Key),
                    Price = value.Value
                });
            }

            return stock;
        }
    }
}
