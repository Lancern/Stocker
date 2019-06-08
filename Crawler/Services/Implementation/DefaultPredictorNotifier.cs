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
        /// <param name="predictorHost">预测器所在的主机。</param>
        /// <param name="httpClientFactory">
        /// 用于创建 <see cref="HttpClient"/> 对象的 <see cref="IHttpClientFactory"/> 对象。
        /// </param>
        public DefaultPredictorNotifier(string predictorHost, 
                                        IHttpClientFactory httpClientFactory,
                                        ILogger<DefaultPredictorNotifier> logger)
        {
            _predictorHost = predictorHost ?? throw new ArgumentNullException(nameof(predictorHost));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <inheritdoc />
        public async Task Notify()
        {
            var url = $"http://{_predictorHost}/updateDB";
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("预测节点返回了异常的 HTTP 状态码：" + response.StatusCode);
                }
            }
        }
    }
}
