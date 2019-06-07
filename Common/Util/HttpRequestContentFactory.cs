using System;
using System.Net.Http;
using System.Text;

namespace Stocker.Util
{
    /// <summary>
    /// 为 <see cref="HttpContent"/> 提供静态工厂方法。
    /// </summary>
    public static class HttpRequestContentFactory
    {
        /// <summary>
        /// 创建包含 JSON 片段的 <see cref="HttpContent"/> 对象。
        /// </summary>
        /// <param name="json">要包含到 <see cref="HttpContent"/> 对象中的 JSON 片段。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="json"/>为null</exception>
        public static HttpContent CreateJsonContent(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
