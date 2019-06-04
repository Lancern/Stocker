using System;
using System.Net.Http;

namespace Stocker.HBase.Implementations
{
    /// <summary>
    /// 提供 <see cref="IHBaseClientFactory"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultHBaseClientFactory : IHBaseClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 初始化 <see cref="DefaultHBaseClientFactory"/> 类的新实例。
        /// </summary>
        /// <param name="clientFactory">用于创建访问 HBase REST API 的 <see cref="HttpClient"/> 对象的工厂对象。</param>
        public DefaultHBaseClientFactory(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        
        /// <inheritdoc />
        public IHBaseClient Create(string address, int port)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));

            var baseAddressBuilder = new UriBuilder { Host = address, Port = port };

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = baseAddressBuilder.Uri;
            
            return new DefaultHBaseClient(httpClient);
        }
    }
}
