using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Stocker.HBase;

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

        /// <summary>
        /// 从 HBase 数据创建 <see cref="StockDailyInfo"/> 封装。
        /// </summary>
        /// <param name="dayCells">日统计数据。</param>
        /// <param name="predictCells">预测数据。</param>
        /// <returns></returns>
        public static Dictionary<long, StockDailyInfo> FromHBaseRowCellCollection(
            HBaseRowCellCollection dayCells,
            HBaseRowCellCollection predictCells)
        {
            var openPrice = PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "open");
            var closePrice = PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "close");
            var lowestPrice = PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "lowest");
            var highestPrice = PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "highest");
            var totalDeals = PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "total");
            var amount = PredictedNumber.FromHBaseRowCellCollection(dayCells, predictCells, "amount");

            var dailyInfo = new Dictionary<long, StockDailyInfo>();

            foreach (var price in openPrice)
            {
                if (!dailyInfo.ContainsKey(price.Key))
                {
                    dailyInfo[price.Key] = new StockDailyInfo
                    {
                        Date = new DateTime(price.Key)
                    };
                }
                dailyInfo[price.Key].OpenPrice = price.Value;
            }

            foreach (var price in closePrice)
            {
                if (!dailyInfo.ContainsKey(price.Key))
                {
                    dailyInfo[price.Key] = new StockDailyInfo
                    {
                        Date = new DateTime(price.Key)
                    };
                }
                dailyInfo[price.Key].SettlementPrice = price.Value;
            }

            foreach (var price in lowestPrice)
            {
                if (!dailyInfo.ContainsKey(price.Key))
                {
                    dailyInfo[price.Key] = new StockDailyInfo
                    {
                        Date = new DateTime(price.Key)
                    };
                }
                dailyInfo[price.Key].LowestPrice = price.Value;
            }

            foreach (var price in highestPrice)
            {
                if (!dailyInfo.ContainsKey(price.Key))
                {
                    dailyInfo[price.Key] = new StockDailyInfo
                    {
                        Date = new DateTime(price.Key)
                    };
                }
                dailyInfo[price.Key].HighestPrice = price.Value;
            }

            foreach (var deal in totalDeals)
            {
                if (!dailyInfo.ContainsKey(deal.Key))
                {
                    dailyInfo[deal.Key] = new StockDailyInfo
                    {
                        Date = new DateTime(deal.Key)
                    };
                }
                dailyInfo[deal.Key].TotalDeals = deal.Value;
            }

            foreach (var deal in amount)
            {
                if (!dailyInfo.ContainsKey(deal.Key))
                {
                    dailyInfo[deal.Key] = new StockDailyInfo
                    {
                        Date = new DateTime(deal.Key)
                    };
                }
                dailyInfo[deal.Key].Amount = deal.Value;
            }

            return dailyInfo;
        }
    }
}
