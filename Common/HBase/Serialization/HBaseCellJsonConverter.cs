using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stocker.HBase.Serialization
{
    /// <summary>
    /// 为 <see cref="HBaseCell"/> 提供 JSON 序列化逻辑。
    /// </summary>
    public sealed class HBaseCellJsonConverter : JsonConverter<HBaseCell>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, HBaseCell value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteStartObject();
            
            writer.WritePropertyName("column");
            serializer.Serialize(writer, value.Column);
            
            writer.WritePropertyName("timestamp");
            writer.WriteValue(value.Timestamp);
            
            writer.WritePropertyName("$", false);
            writer.WriteValue(Convert.ToBase64String(value.Data));
            
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override HBaseCell ReadJson(JsonReader reader, Type objectType, HBaseCell existingValue, 
                                           bool hasExistingValue, JsonSerializer serializer)
        {
            var valueJsonObject = JToken.Load(reader);

            if (!hasExistingValue)
            {
                existingValue = new HBaseCell();
            }

            existingValue.Column = valueJsonObject["column"]?.ToObject<HBaseColumn>(serializer);
            existingValue.Timestamp = valueJsonObject["timestamp"]?.Value<int>() ?? 0;

            var dataBase64 = valueJsonObject["$"]?.Value<string>();
            if (dataBase64 != null)
            {
                existingValue.Data = Convert.FromBase64String(dataBase64);
            }

            return existingValue;
        }
    }
}
