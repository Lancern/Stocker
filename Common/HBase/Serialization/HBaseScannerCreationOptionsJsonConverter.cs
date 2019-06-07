using System;
using System.Text;
using Newtonsoft.Json;

namespace Stocker.HBase.Serialization
{
    /// <summary>
    /// 为 <see cref="HBaseScannerCreationOptions"/> 提供 JSON 序列化逻辑。
    /// </summary>
    public sealed class HBaseScannerCreationOptionsJsonConverter : JsonConverter<HBaseScannerCreationOptions>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, HBaseScannerCreationOptions value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (value.Columns != null)
            {
                writer.WritePropertyName("column");
                serializer.Serialize(writer, value.Columns);
            }

            if (value.StartRowKey != null)
            {
                writer.WritePropertyName("startRow");
                serializer.Serialize(writer, Convert.ToBase64String(Encoding.UTF8.GetBytes(value.StartRowKey)));
            }

            if (value.EndRowKey != null)
            {
                writer.WritePropertyName("endRow");
                serializer.Serialize(writer, Convert.ToBase64String(Encoding.UTF8.GetBytes(value.EndRowKey)));
            }

            if (value.Batch != null)
            {
                writer.WritePropertyName("batch");
                writer.WriteValue(value.Batch.Value);
            }

            if (value.StartTime != null)
            {
                writer.WritePropertyName("startTime");
                writer.WriteValue(value.StartTime.Value);
            }

            if (value.EndTime != null)
            {
                writer.WritePropertyName("endTime");
                writer.WriteValue(value.EndTime.Value);
            }

            if (value.MaxVersions != null)
            {
                writer.WritePropertyName("maxVersions");
                writer.WriteValue(value.MaxVersions.Value);
            }
        }

        /// <inheritdoc />
        public override HBaseScannerCreationOptions ReadJson(JsonReader reader, Type objectType, HBaseScannerCreationOptions existingValue,
                                                             bool hasExistingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException($"{nameof(HBaseScannerCreationOptions)}对象不应该被反序列化。");
        }
    }
}
