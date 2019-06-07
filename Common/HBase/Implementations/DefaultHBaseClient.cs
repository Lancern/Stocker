using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stocker.HBase.Serialization;
using Stocker.Util;

namespace Stocker.HBase.Implementations
{
    /// <summary>
    /// 提供 <see cref="IHBaseClient"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultHBaseClient : IHBaseClient
    {
        private const int DefaultScannerBatchSize = 100;
        
        /// <summary>
        /// 为发送到 HBase REST API 的数据行数据提供外围数据包装。
        /// </summary>
        private sealed class HBaseRowWrapper
        {
            /// <summary>
            /// 获取或设置所有的数据行。
            /// </summary>
            [JsonProperty("Row")]
            public List<HBaseRow> Rows { get; set; }
        }

        private readonly string _address;
        private readonly int _port;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool _disposed;
        
        /// <summary>
        /// 初始化 <see cref="DefaultHBaseClient"/> 类的新实例。
        /// </summary>
        /// <param name="address">HBase 实例所在主机的地址。</param>
        /// <param name="port">HBase 实例所在的端口号。</param>
        /// <param name="httpClientFactory">用于创建访问 HBase REST API 的 <see cref="HttpClient"/> 的工厂对象。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="address"/>为null
        ///     或
        ///     <paramref name="httpClientFactory"/>为null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="port"/>小于0 或 大于65535
        /// </exception>
        public DefaultHBaseClient(string address, int port, IHttpClientFactory httpClientFactory)
        {
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));

            _address = address ?? throw new ArgumentNullException(nameof(address));
            _port = port;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _disposed = false;
        }

        /// <summary>
        /// 创建新的 <see cref="HttpClient"/> 对象。
        /// </summary>
        /// <returns>创建的 <see cref="HttpClient"/> 对象。</returns>
        private HttpClient CreateHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new UriBuilder
            {
                Host = _address,
                Port = _port
            }.Uri;
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            return httpClient;
        }

        /// <summary>
        /// 当当前对象已经被 Dispose 时抛出 <see cref="ObjectDisposedException"/> 异常。
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
        
        /// <inheritdoc />
        public async Task Add(string tableName, IEnumerable<HBaseRow> rows)
        {
            EnsureNotDisposed();
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            var wrapper = new HBaseRowWrapper { Rows = rows.ToList() };
            var json = HBaseSerializationHelper.SerializeObject(wrapper);

            var url = $"{tableName}/row_key";
            var content = HttpRequestContentFactory.CreateJsonContent(json);

            HttpResponseMessage response;
            using (var httpClient = CreateHttpClient())
            {
                response = await httpClient.PutAsync(url, content);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HBaseException("HBase REST API 返回了表示错误的 HTTP 状态码：" + response.StatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<HBaseRow> Find(string tableName, string rowKey, HBaseFindOptions options = null)
        {
            EnsureNotDisposed();
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            if (rowKey == null)
                throw new ArgumentNullException(nameof(rowKey));

            var urlBuilder = new StringBuilder($"{tableName}/{rowKey}");
            if (options?.Column != null)
            {
                urlBuilder.AppendFormat("/{0}", options.Column);
            }

            if (options?.Timestamp != null)
            {
                urlBuilder.AppendFormat("/{0}", options.Timestamp);
            }

            if (options?.NumberOfVersions != null)
            {
                urlBuilder.AppendFormat("?v={0}", options.NumberOfVersions);
            }

            var url = urlBuilder.ToString();

            HttpResponseMessage response;
            using (var httpClient = CreateHttpClient())
            {
                response = await httpClient.GetAsync(url);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HBaseException("HBase REST API 返回了表示错误的 HTTP 状态码：" + response.StatusCode);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var data = HBaseSerializationHelper.DeserializeObject<HBaseRowWrapper>(responseBody);
            if (data.Rows == null || data.Rows.Count == 0)
            {
                return null;
            }

            return data.Rows[0];
        }

        /// <inheritdoc />
        public async Task<IHBaseScanner> OpenScanner(string tableName, HBaseScannerCreationOptions options = null)
        {
            EnsureNotDisposed();
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));

            // 构造请求负载
            var scannerOptionsJson = new JObject();
            if (options?.Columns != null)
            {
                scannerOptionsJson.Add("column", HBaseSerializationHelper.ToJToken(options.Columns));
            }

            if (options?.StartRowKey != null)
            {
                var startRowKeyBytes = Encoding.UTF8.GetBytes(options.StartRowKey);
                scannerOptionsJson.Add("startRow", JToken.FromObject(Convert.ToBase64String(startRowKeyBytes)));
            }

            if (options?.EndRowKey != null)
            {
                var endRowKeyBytes = Encoding.UTF8.GetBytes(options.EndRowKey);
                scannerOptionsJson.Add("endRow", JToken.FromObject(Convert.ToBase64String(endRowKeyBytes)));
            }

            if (options?.StartTime != null)
            {
                scannerOptionsJson.Add("startTime", JToken.FromObject(options.StartTime));
            }

            if (options?.EndTime != null)
            {
                scannerOptionsJson.Add("endTime", JToken.FromObject(options.EndTime));
            }

            scannerOptionsJson.Add("batch", JToken.FromObject(options?.Batch ?? DefaultScannerBatchSize));

            var contentJson = HBaseSerializationHelper.SerializeJToken(scannerOptionsJson);

            // 发送 HTTP 请求
            var url = $"{tableName}/scanner";
            var content = HttpRequestContentFactory.CreateJsonContent(contentJson);

            HttpResponseMessage response;
            using (var httpClient = CreateHttpClient())
            {
                response = await httpClient.PutAsync(url, content);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HBaseException("HBase REST API 返回了表示错误的 HTTP 状态码：" + response.StatusCode);
            }

            // 从响应的 Location 头部拿到 Scanner 的接入点
            if (!response.Headers.TryGetValues("Location", out var scannerEndpoints))
            {
                throw new HBaseException("HBase REST API 没有返回任何有效的 Scanner 终结点。");
            }

            var scannerEpt = scannerEndpoints.FirstOrDefault();
            if (scannerEpt == null)
            {
                throw new HBaseException("HBase REST API 没有返回任何有效的 Scanner 终结点。");
            }

            return new DefaultHBaseScanner(scannerEpt, _httpClientFactory);
        }
    }
}
