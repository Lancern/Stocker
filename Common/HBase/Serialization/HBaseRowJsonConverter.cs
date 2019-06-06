using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stocker.HBase.Serialization
{
    /// <summary>
    /// 为 <see cref="HBaseRow"/> 提供 JSON 序列化逻辑。
    /// </summary>
    public sealed class HBaseRowJsonConverter : JsonConverter<HBaseRow>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, HBaseRow value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteStartObject();
            
            writer.WritePropertyName("key");
            writer.WriteValue(Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Key)));
            
            writer.WritePropertyName("Cell");
            serializer.Serialize(writer, value.Cells);
            
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override HBaseRow ReadJson(JsonReader reader, Type objectType, HBaseRow existingValue, bool hasExistingValue,
                                          JsonSerializer serializer)
        {
            var valueJsonObject = JToken.Load(reader);

            if (!hasExistingValue)
            {
                existingValue = new HBaseRow();
            }

            var keyBase64 = valueJsonObject["key"].Value<string>();
            if (keyBase64 != null)
            {
                existingValue.Key = Encoding.UTF8.GetString(Convert.FromBase64String(keyBase64));
            }

            var cells = valueJsonObject["Cell"]?.ToObject<List<HBaseCell>>(serializer);
            if (cells != null)
            {
                foreach (var c in cells)
                {
                    existingValue.Cells.Add(c);
                }
            }

            return existingValue;
        }
    }
}
