using Newtonsoft.Json;
using Stocker.WebAPI.Models.Utils;

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
    }
}
