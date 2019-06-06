using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Stocker.HBase.Implementations
{
    /// <summary>
    /// 提供 <see cref="IHBaseClientFactory"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultHBaseClientFactory : IHBaseClientFactory
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 初始化 <see cref="DefaultHBaseClientFactory"/> 类的新实例。
        /// </summary>
        /// <param name="config">应用程序配置根</param>
        /// <param name="clientFactory">用于创建访问 HBase REST API 的 <see cref="HttpClient"/> 对象的工厂对象。</param>
        public DefaultHBaseClientFactory(IConfiguration config, IHttpClientFactory clientFactory)
        {
            _config = config;
            _httpClientFactory = clientFactory;
        }

        /// <inheritdoc />
        public IHBaseClient Create()
        {
            var hbaseSection = _config.GetSection("HBase");
            if (!hbaseSection.Exists())
            {
                throw new InvalidOperationException("在应用程序配置根中未能找到 HBase 相关配置项。");
            }

            var address = hbaseSection.GetValue<string>("Address");
            var port = hbaseSection.GetValue<int>("port");
            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("在应用程序配置根中未能找到 HBase 实例地址。");
            }

            return Create(address, port);
        }

        /// <inheritdoc />
        public IHBaseClient Create(string address, int port)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));
            
            return new DefaultHBaseClient(address, port, _httpClientFactory);
        }
    }
}
