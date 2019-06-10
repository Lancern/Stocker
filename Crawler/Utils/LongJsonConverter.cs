using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stocker.Crawler.Utils
{
    /// <summary>
    /// 提供可从浮点数抽取整数的 <see cref="JsonConverter{Int64}"/> 实现。
    /// </summary>
    public sealed class LongJsonConverter : JsonConverter<long>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        /// <inheritdoc />
        public override long ReadJson(JsonReader reader, Type objectType, long existingValue, bool hasExistingValue, 
                                      JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            try
            {
                existingValue = token.Value<long>();
            }
            catch
            {
                existingValue = (long) token.Value<double>();
            }
            
            return existingValue;
        }
    }
}
