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
        public static IEnumerable<KeyValuePair<long, PredictedNumber>> FromHBaseRowCellCollection(
            HBaseRowCellCollection dayCells,
            HBaseRowCellCollection predictCells,
            string columnName)
        {
            if (dayCells == null)
                throw new ArgumentNullException(nameof(dayCells));

            var pdNum = new Dictionary<long, PredictedNumber>();

            foreach (var actualValue in dayCells.Get(columnName, columnName))
            {
                if (!pdNum.ContainsKey(actualValue.Timestamp))
                    pdNum[actualValue.Timestamp] = new PredictedNumber();
                pdNum[actualValue.Timestamp].ActualValue =
                    double.Parse(Encoding.UTF8.GetString(actualValue.Data));
            }

            foreach (var predictedMinValue in predictCells.Get(columnName, "lower"))
            {
                if (!pdNum.ContainsKey(predictedMinValue.Timestamp))
                    pdNum[predictedMinValue.Timestamp] = new PredictedNumber();
                pdNum[predictedMinValue.Timestamp].PredictedMinValue =
                    double.Parse(Encoding.UTF8.GetString(predictedMinValue.Data));
            }

            foreach (var predictedValue in predictCells.Get(columnName, "value"))
            {
                if (!pdNum.ContainsKey(predictedValue.Timestamp))
                    pdNum[predictedValue.Timestamp] = new PredictedNumber();
                pdNum[predictedValue.Timestamp].PredictedValue =
                    double.Parse(Encoding.UTF8.GetString(predictedValue.Data));
            }

            foreach (var predictedMaxValue in predictCells.Get(columnName, "upper"))
            {
                if (!pdNum.ContainsKey(predictedMaxValue.Timestamp))
                    pdNum[predictedMaxValue.Timestamp] = new PredictedNumber();
                pdNum[predictedMaxValue.Timestamp].PredictedMaxValue =
                    double.Parse(Encoding.UTF8.GetString(predictedMaxValue.Data));
            }

            return pdNum;
        }
    }
}
