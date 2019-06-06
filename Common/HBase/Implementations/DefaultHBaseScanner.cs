using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stocker.HBase.Serialization;

namespace Stocker.HBase.Implementations
{
    /// <summary>
    /// 提供 <see cref="IHBaseScanner"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultHBaseScanner : IHBaseScanner
    {
        /// <summary>
        /// 为从 HBase REST API 接收到的数据行数据提供外围数据包装。
        /// </summary>
        private sealed class HBaseRowWrapper
        {
            /// <summary>
            /// 获取或设置所有的数据行。
            /// </summary>
            [JsonProperty("Row")]
            public List<HBaseRow> Rows { get; set; }
        }
        
        private readonly HttpClient _httpClient;
        private List<HBaseRow> _batch;
        private bool _disposed;

        /// <summary>
        /// 初始化 <see cref="DefaultHBaseScanner"/> 的新实例。
        /// </summary>
        /// <param name="endpoint">Scanner 的 REST API 接入点。</param>
        /// <param name="httpClientFactory">用于创建发送和接收 HTTP 请求的 <see cref="HttpClient"/> 对象的工厂。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="endpoint"/>为null
        ///     或
        ///     <paramref name="httpClientFactory"/>为null
        /// </exception>
        public DefaultHBaseScanner(string endpoint, IHttpClientFactory httpClientFactory)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));
            
            _httpClient = httpClientFactory.CreateClient();
            _batch = null;
            _disposed = false;
            
            InitializeHttpClient(endpoint);
        }

        /// <summary>
        /// 初始化 <see cref="HttpClient"/> 实例以用于发送和接收 HTTP 请求。
        /// </summary>
        /// <param name="endpoint">当前 Scanner 的 REST API 接入点。</param>
        private void InitializeHttpClient(string endpoint)
        {
            _httpClient.BaseAddress = new Uri(endpoint);
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/json");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "text/json");
        }

        ~DefaultHBaseScanner()
        {
            Dispose(false);
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <inheritdoc />
        public List<HBaseRow> CurrentBatch
        {
            get
            {
                EnsureNotDisposed();
                return _batch;
            }
        }
        
        /// <inheritdoc />
        public async Task<bool> ReadNextBatch()
        {
            EnsureNotDisposed();

            var response = await _httpClient.GetAsync(string.Empty);
            if (!response.IsSuccessStatusCode)
            {
                throw new HBaseException("HBase 返回了表示操作不成功的 HTTP 状态码：" + response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                // 返回的 HTTP 状态码为 204 No Content
                _batch = null;
                return false;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = HBaseSerializationHelper.DeserializeObject<HBaseRowWrapper>(responseBody);
            _batch = responseObject.Rows;

            return true;
        }

        /// <summary>
        /// 从 HBase 实例中删除当前的 Scanner。
        /// </summary>
        /// <returns></returns>
        private async Task DeleteScanner()
        {
            EnsureNotDisposed();

            var response = await _httpClient.DeleteAsync(string.Empty);
            if (!response.IsSuccessStatusCode)
            {
                throw new HBaseException("HBase 返回了表示操作不成功的 HTTP 状态码：" + response.StatusCode);
            }
        }

        /// <summary>
        /// 释放当前对象所占有的全部外部资源。
        /// </summary>
        /// <param name="disposing">用户是否正在调用 <see cref="Dispose()"/> 方法。</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        DeleteScanner().Wait();
                    }
                    catch
                    {
                        // 丢弃 DeleteScanner 抛出的所有异常
                    }
                    
                    _httpClient.Dispose();
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
