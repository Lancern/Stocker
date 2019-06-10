using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Stocker.Crawler.Services.Implementation
{
    /// <summary>
    /// 为 <see cref="IStockInfoProvider"/> 提供默认实现。
    /// </summary>
    internal sealed class DefaultStockInfoProvider : IStockInfoProvider
    {
        private const string UrlTemplate = "https://api.shenjian.io/?appid={0}";

        private static readonly SemaphoreSlim ConcurrentRequestLock;
        private static readonly SemaphoreSlim SendRequestLock;

        static DefaultStockInfoProvider()
        {
            ConcurrentRequestLock = new SemaphoreSlim(3, 3);
            SendRequestLock = new SemaphoreSlim(1, 1);
        }

        private static string CreateRequestUrl(string appId)
        {
            return string.Format(UrlTemplate, appId);
        }

        private readonly ILogger<DefaultStockInfoProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _stockListAppId;
        private readonly string _stockDailyStatisticsAppId;

        /// <summary>
        /// 初始化 <see cref="DefaultStockInfoProvider"/> 类的新实例。
        /// </summary>
        /// <param name="logger">日志支撑件。</param>
        /// <param name="httpClientFactory">用于创建发送 HTTP 请求的对象的工厂。</param>
        /// <param name="stockListAppId">用于访问股票列表 API 的 App ID。</param>
        /// <param name="stockDailyStatisticsAppId">用于访问股票日交易数据 API 的 App ID。</param>
        public DefaultStockInfoProvider(ILogger<DefaultStockInfoProvider> logger, IHttpClientFactory httpClientFactory, 
                                        string stockListAppId, string stockDailyStatisticsAppId)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _stockListAppId = stockListAppId;
            _stockDailyStatisticsAppId = stockDailyStatisticsAppId;
        }
        
        /// <summary>
        /// 从指定的 URL 处获取 HTTP 响应。全局并发控制策略以及请求间隔策略将会被应用。
        /// </summary>
        /// <param name="url">要请求的目标 URL。</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetResponse(string url)
        {
            await ConcurrentRequestLock.WaitAsync();
            await SendRequestLock.WaitAsync();
            await Task.Delay(500); // 等待 500 毫秒防止过于频繁的请求
            SendRequestLock.Release();

            var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.GetAsync(url)
                                   .ContinueWith(t =>
                                   {
                                       httpClient.Dispose();
                                       ConcurrentRequestLock.Release();
                                       return t.Result;
                                   });
        }

        /// <inheritdoc />
        public async Task<List<StockRealtimeInfo>> GetRealtimeStocksList()
        {
            var url = CreateRequestUrl(_stockListAppId);
            var response = await GetResponse(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("获取股票实时列表信息时 API 返回了异常的 HTTP 状态码 {0}", response.StatusCode);
                return new List<StockRealtimeInfo>();
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var responseJson = JObject.Parse(responseBody);
            if (responseJson.TryGetValue("error_code", out var errorCodeToken) && errorCodeToken.Value<int>() != 0)
            {
                var message = string.Empty;
                if (responseJson.TryGetValue("message", out var messageToken))
                {
                    message = messageToken.Value<string>();
                }

                _logger.LogError("获取股票实时列表信息时 API 返回错误码：{0}：{1}", 
                                 errorCodeToken.Value<int>(), message);
                return new List<StockRealtimeInfo>();
            }

            if (!responseJson.TryGetValue("data", out var dataToken))
            {
                _logger.LogWarning("获取股票实时列表信息时 API 返回的数据不包含 data 字段。");
                return new List<StockRealtimeInfo>();
            }

            return dataToken.ToObject<List<StockRealtimeInfo>>();
        }

        /// <inheritdoc />
        public async Task<StockDailyStatisticsInfo> GetDailyStatisticsInfo(string code, DateTime startDate, DateTime endDate)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            
            startDate = startDate.Date;
            endDate = endDate.Date;
            if (startDate > endDate)
                throw new ArgumentException($"{nameof(startDate)}不能晚于{nameof(endDate)}");

            var urlBuilder = new StringBuilder(CreateRequestUrl(_stockDailyStatisticsAppId));
            urlBuilder.AppendFormat("&code={0}&start_date={1:yyyy-MM-dd}&end_date={2:yyyy-MM-dd}", 
                                    code, startDate, endDate);
            urlBuilder.Append("&index=false&k_type=day&fq_type=qfq");
            var url = urlBuilder.ToString();

            var response = await GetResponse(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("获取股票日统计数据时 API 返回了异常的 HTTP 状态码：" + response.StatusCode);
                return null;
            }
            
            var responseBody = await response.Content.ReadAsStringAsync();

            var responseJson = JObject.Parse(responseBody);
            if (responseJson.TryGetValue("error_code", out var errorCodeToken) && errorCodeToken.Value<int>() != 0)
            {
                var message = string.Empty;
                if (responseJson.TryGetValue("message", out var messageToken))
                {
                    message = messageToken.Value<string>();
                }

                _logger.LogError("获取股票日统计数据时 API 返回了错误码：{0}：{1}",
                                 errorCodeToken.Value<int>(), message);
                return null;
            }

            if (!responseJson.TryGetValue("data", out var dataToken))
            {
                _logger.LogError("获取股票日统计数据时 API 返回的内容不包含 data 成员。");
                return null;
            }

            return dataToken.ToObject<StockDailyStatisticsInfo>();
        }
    }
}
