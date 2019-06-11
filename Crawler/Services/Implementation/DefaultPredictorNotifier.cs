using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stocker.Crawler.Services.Implementation
{
    /// <summary>
    /// 提供 <see cref="IPredictorNotifier"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultPredictorNotifier : IPredictorNotifier
    {
        private readonly string _predictorHost;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DefaultPredictorNotifier> _logger;

        /// <summary>
        /// 初始化 <see cref="DefaultPredictorNotifier"/> 的新实例。
        /// </summary>
        /// <param name="predictorEndpoint">预测器所在的主机。</param>
        /// <param name="httpClientFactory">
        /// 用于创建 <see cref="HttpClient"/> 对象的 <see cref="IHttpClientFactory"/> 对象。
        /// </param>
        /// <param name="logger">日志支撑件</param>
        public DefaultPredictorNotifier(string predictorEndpoint, 
                                        IHttpClientFactory httpClientFactory,
                                        ILogger<DefaultPredictorNotifier> logger)
        {
            _predictorHost = predictorEndpoint ?? throw new ArgumentNullException(nameof(predictorEndpoint));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <inheritdoc />
        public async Task Notify()
        {
            var url = $"http://{_predictorHost}/updateDB";
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.PostAsync(url, new ByteArrayContent(Array.Empty<byte>()));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("预测节点返回了异常的 HTTP 状态码：" + response.StatusCode);
                }
            }
        }
    }
}
