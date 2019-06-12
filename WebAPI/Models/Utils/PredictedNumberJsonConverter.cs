using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stocker.WebAPI.Models.Utils
{
    /// <summary>
    /// 为 <see cref="PredictedNumber"/> 提供
    /// </summary>
    public sealed class PredictedNumberJsonConverter : JsonConverter<PredictedNumber>
    {
        /// <summary>
        /// 提供需要被序列化的 <see cref="PredictedNumber"/> 成员。
        /// </summary>
        private static readonly Dictionary<string, PropertyInfo> TargetProperties;

        static PredictedNumberJsonConverter()
        {
            TargetProperties = new Dictionary<string, PropertyInfo>
            {
                { "actualValue", typeof(PredictedNumber).GetProperty(nameof(PredictedNumber.ActualValue)) },
                { "predictedMinValue", typeof(PredictedNumber).GetProperty(nameof(PredictedNumber.PredictedMinValue)) },
                { "predictedValue", typeof(PredictedNumber).GetProperty(nameof(PredictedNumber.PredictedValue)) },
                { "predictedMaxValue", typeof(PredictedNumber).GetProperty(nameof(PredictedNumber.PredictedMaxValue)) }
            };
        }
        
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, PredictedNumber value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteStartObject();

            foreach (var (propertyName, property) in TargetProperties)
            {
                if (property.GetValue(value) is double propertyValue)
                {
                    writer.WritePropertyName(propertyName);
                    writer.WriteValue(propertyValue);
                }
            }
            
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override PredictedNumber ReadJson(JsonReader reader, Type objectType, PredictedNumber existingValue, 
                                                 bool hasExistingValue, JsonSerializer serializer)
        {
            if (!hasExistingValue)
            {
                existingValue = new PredictedNumber();
            }

            var jsonLayout = JObject.Load(reader);
            foreach (var (key, value) in jsonLayout)
            {
                if (!TargetProperties.TryGetValue(key, out var property))
                {
                    continue;
                }

                property.SetValue(existingValue, value.Value<double?>());
            }

            return existingValue;
        }
    }
}
