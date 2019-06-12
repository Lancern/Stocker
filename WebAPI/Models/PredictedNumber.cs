using Newtonsoft.Json;
using Stocker.HBase;
using Stocker.WebAPI.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocker.WebAPI.Models
{
    /// <summary>
    /// 封装实际值与预测值。
    /// </summary>
    [JsonConverter(typeof(PredictedNumberJsonConverter))]
    public sealed class PredictedNumber
    {
        /// <summary>
        /// 获取或设置实际值。
        /// </summary>
        public double? ActualValue { get; set; }
        
        /// <summary>
        /// 获取或设置预测的最小值。
        /// </summary>
        public double? PredictedMinValue { get; set; }
        
        /// <summary>
        /// 获取或设置预测值。
        /// </summary>
        public double? PredictedValue { get; set; }
        
        /// <summary>
        /// 获取或设置预测的最大值。
        /// </summary>
        public double? PredictedMaxValue { get; set; }

        /// <summary>
        /// 将day表中的真实值与prediction表中的预测值合并。
        /// </summary>
        /// <param name="dayCells">day表中符合条件的cell</param>
        /// <param name="predictCells">prediction表中符合条件的cell</param>
        /// <param name="columnName">要合并的列族名</param>
        /// <returns></returns>
        public static Dictionary<DateTime, PredictedNumber> FromHBaseRowCellCollection(
            HBaseRowCellCollection dayCells,
            HBaseRowCellCollection predictCells,
            string columnName)
        {
            if (dayCells == null)
                throw new ArgumentNullException(nameof(dayCells));

            if (predictCells == null)
                throw new ArgumentNullException(nameof(predictCells));

            var pdNum = new Dictionary<DateTime, PredictedNumber>();

            foreach (var actualValue in dayCells.Get(columnName, columnName))
            {
                var date = new DateTime(actualValue.Timestamp).Date;
                if (!pdNum.ContainsKey(date))
                {
                    pdNum[date] = new PredictedNumber();
                }
                
                pdNum[date].ActualValue =
                    double.Parse(Encoding.UTF8.GetString(actualValue.Data));
            }

            foreach (var predictedMinValue in predictCells.Get(columnName, "lower"))
            {
                var date = new DateTime(predictedMinValue.Timestamp).Date;
                if (!pdNum.ContainsKey(date))
                {
                    pdNum[date] = new PredictedNumber();
                }
                
                pdNum[date].PredictedMinValue =
                    double.Parse(Encoding.UTF8.GetString(predictedMinValue.Data));
            }

            foreach (var predictedValue in predictCells.Get(columnName, "value"))
            {
                var date = new DateTime(predictedValue.Timestamp).Date;
                if (!pdNum.ContainsKey(date))
                {
                    pdNum[date] = new PredictedNumber();
                }
                
                pdNum[date].PredictedValue =
                    double.Parse(Encoding.UTF8.GetString(predictedValue.Data));
            }

            foreach (var predictedMaxValue in predictCells.Get(columnName, "upper"))
            {
                var date = new DateTime(predictedMaxValue.Timestamp).Date;
                if (!pdNum.ContainsKey(date))
                {
                    pdNum[date] = new PredictedNumber();
                }
                
                pdNum[date].PredictedMaxValue =
                    double.Parse(Encoding.UTF8.GetString(predictedMaxValue.Data));
            }

            return pdNum;
        }
    }
}
