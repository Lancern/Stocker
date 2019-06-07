using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Stocker.HBase;

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

        /// <summary>
        /// 从给定的 HBase 数据行创建 <see cref="StockListItem"/> 对象。
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/>为null</exception>
        public static StockListItem FromHBaseRow(HBaseRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            var stock = new StockListItem { Code = row.Key };

            var nameCells = row.Cells.Get("name", "name").ToArray();
            if (nameCells.Length > 0)
            {
                stock.Name = Encoding.UTF8.GetString(nameCells[0].Data);
            }

            var priceCells = row.Cells.Get("price", "price").ToArray();
            if (priceCells.Length > 0)
            {
                stock.Price = double.Parse(Encoding.UTF8.GetString(priceCells[0].Data));
            }

            var priceChangeCells = row.Cells.Get("priceChange", "priceChange").ToArray();
            if (priceChangeCells.Length > 0)
            {
                stock.PriceRelativeChangePercent = double.Parse(Encoding.UTF8.GetString(priceChangeCells[0].Data));
            }

            var updateTimeCells = row.Cells.Get("date", "date").ToArray();
            if (updateTimeCells.Length > 0)
            {
                stock.UpdateTime = DateTime.Parse(Encoding.UTF8.GetString(updateTimeCells[0].Data));
            }

            return stock;
        }
    }
}
