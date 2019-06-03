using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Stocker.Crawler.Services.Implementation
{
    /// <summary>
    /// 为 <see cref="ICrawler"/> 提供默认实现。
    /// </summary>
    internal sealed class DefaultCrawler : ICrawler
    {
        private const string UrlTemplate = "https://api.shenjian.io/?appid={0}";

        private static string CreateRequestUrl(string appId)
        {
            return string.Format(UrlTemplate, appId);
        }

        private readonly ILogger<DefaultCrawler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _appId;

        /// <summary>
        /// 初始化 <see cref="DefaultCrawler"/> 类的新实例。
        /// </summary>
        /// <param name="logger">日志支撑件。</param>
        /// <param name="httpClientFactory">用于创建发送 HTTP 请求的对象的工厂。</param>
        /// <param name="appId">用于访问神箭数据源的 App ID。</param>
        public DefaultCrawler(ILogger<DefaultCrawler> logger, IHttpClientFactory httpClientFactory, string appId)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _appId = appId;
        }

        /// <inheritdoc />
        public async Task<List<StockInfo>> GetStocksList()
        {
            var url = CreateRequestUrl(_appId);
            string responseBody;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("神箭数据源接口返回了异常状态码 {0}", response.StatusCode);
                    return new List<StockInfo>();
                }

                responseBody = await response.Content.ReadAsStringAsync();
            }

            var responseJson = JObject.Parse(responseBody);
            if (responseJson.TryGetValue("error_code", out var errorCodeToken) && errorCodeToken.Value<int>() != 0)
            {
                var message = string.Empty;
                if (responseJson.TryGetValue("message", out var messageToken))
                {
                    message = messageToken.Value<string>();
                }

                _logger.LogError("神箭数据源接口返回错误码：{0}：{1}", errorCodeToken.Value<int>(), message);
                return new List<StockInfo>();
            }

            if (!responseJson.TryGetValue("data", out var dataToken))
            {
                _logger.LogWarning("神箭数据源接口返回的数据不包含 data 字段。");
                return new List<StockInfo>();
            }

            return dataToken.ToObject<List<StockInfo>>();
        }
    }
}
