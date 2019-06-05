using System;
using System.Text;
using Newtonsoft.Json;

namespace Stocker.HBase.Serialization
{
    /// <summary>
    /// 为 <see cref="HBaseColumn"/> 提供 JSON 序列化逻辑。
    /// </summary>
    public sealed class HBaseColumnJsonConverter : JsonConverter<HBaseColumn>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, HBaseColumn value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            var valueBytes = Encoding.UTF8.GetBytes(value.ToString());
            writer.WriteValue(Convert.ToBase64String(valueBytes));
        }

        /// <inheritdoc />
        public override HBaseColumn ReadJson(JsonReader reader, Type objectType, HBaseColumn existingValue, 
                                             bool hasExistingValue, JsonSerializer serializer)
        {
            var base64 = reader.ReadAsString();
            var column = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var splitterIndex = column.IndexOf(':');

            if (splitterIndex == -1)
            {
                return new HBaseColumn(column, string.Empty);
            }
            else
            {
                return new HBaseColumn(column.Substring(0, splitterIndex),
                                       column.Substring(splitterIndex + 1));
            }
        }
    }
}
