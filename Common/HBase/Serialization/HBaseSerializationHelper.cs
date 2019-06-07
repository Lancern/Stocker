using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stocker.HBase.Serialization
{
    /// <summary>
    /// 为带有 HBase 数据模型的对象图提供到 JSON 的序列化服务。
    /// </summary>
    public static class HBaseSerializationHelper
    {
        private static readonly JsonConverter[] HBaseConverters = 
        {
            new HBaseColumnJsonConverter(), 
            new HBaseCellJsonConverter(), 
            new HBaseRowJsonConverter(),
            new HBaseScannerCreationOptionsJsonConverter()
        };

        /// <summary>
        /// 获取给定的对象的 JSON 表示。
        /// </summary>
        /// <param name="value">要序列化的对象。</param>
        /// <param name="converters">要额外使用的 <see cref="JsonConverter"/> 对象。</param>
        /// <typeparam name="T">要序列化的对象类型。</typeparam>
        /// <returns></returns>
        public static string SerializeObject<T>(T value, params JsonConverter[] converters)
        {
            var actualConverters = HBaseConverters.Concat(converters).ToArray();
            return JsonConvert.SerializeObject(value, actualConverters);
        }

        /// <summary>
        /// 获取给定的对象的 <see cref="JToken"/> 表示。
        /// </summary>
        /// <param name="value">要序列化的对象。</param>
        /// <param name="converters">要额外使用的 <see cref="JsonConverter"/> 对象。</param>
        /// <typeparam name="T">要序列化的对象类型。</typeparam>
        /// <returns></returns>
        public static JToken ToJToken<T>(T value, params JsonConverter[] converters)
        {
            var serializer = JsonSerializer.CreateDefault();
            foreach (var conv in HBaseConverters.Concat(converters))
            {
                serializer.Converters.Add(conv);
            }

            return JToken.FromObject(value, serializer);
        }

        /// <summary>
        /// 获取给定的 <see cref="JToken"/> 对象的 JSON 表示。
        /// </summary>
        /// <param name="token">要序列化的 <see cref="JToken"/> 对象。</param>
        /// <param name="converters">要额外使用的 <see cref="JsonConverter"/> 对象。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="token"/>为null</exception>
        public static string SerializeJToken(JToken token, params JsonConverter[] converters)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            
            var actualConverters = HBaseConverters.Concat(converters).ToArray();
            return token.ToString(Formatting.None, actualConverters);
        }

        /// <summary>
        /// 从给定的 JSON 片段反序列化给定类型的实例对象。
        /// </summary>
        /// <param name="json">JSON 片段。</param>
        /// <param name="converters">要额外使用的 <see cref="JsonConverter"/> 对象。</param>
        /// <typeparam name="T">要反序列化的对象类型。</typeparam>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json, params JsonConverter[] converters)
        {
            var actualConverters = HBaseConverters.Concat(converters).ToArray();
            return JsonConvert.DeserializeObject<T>(json, actualConverters);
        }

        /// <summary>
        /// 从给定的 <see cref="JToken"/> 对象反序列化给定类型的实例对象。
        /// </summary>
        /// <param name="token"><see cref="JToken"/> 对象。</param>
        /// <param name="converters">要额外使用的 <see cref="JsonConverter"/> 对象。</param>
        /// <typeparam name="T">要反序列化的对象类型。</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="token"/>为null</exception>
        public static T FromJToken<T>(JToken token, params JsonConverter[] converters)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            
            var serializer = JsonSerializer.CreateDefault();
            foreach (var conv in HBaseConverters.Concat(converters))
            {
                serializer.Converters.Add(conv);
            }

            return token.ToObject<T>(serializer);
        }
    }
}
