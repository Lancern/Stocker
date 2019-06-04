using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Stocker.HBase.Implementations
{
    /// <summary>
    /// 提供 <see cref="IHBaseClient"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultHBaseClient : IHBaseClient
    {
        private readonly HttpClient _httpClient;
        private bool _disposed;
        
        /// <summary>
        /// 初始化 <see cref="DefaultHBaseClient"/> 类的新实例。
        /// </summary>
        /// <param name="httpClient">
        /// 用于访问 HBase REST API 的 <see cref="HttpClient"/> 对象。该对象的 <see cref="HttpClient.BaseAddress"/> 属性应该
        /// 包含 HBase REST API 服务的地址。
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/>为null</exception>
        public DefaultHBaseClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _disposed = false;
        }

        ~DefaultHBaseClient()
        {
            Dispose(false);
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

            var url = $"{_httpClient.BaseAddress}/{tableName}/row_key";

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            foreach (var row in rows)
            {
                // TODO: HBaseRow转化为json
                var content = new StringContent(row.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.PutAsync(url, content);
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

            var url = $"{_httpClient.BaseAddress}/{tableName}/{rowKey}";

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                // TODO: 从返回的json转化为HBaseRow
                return new HBaseRow();
            }
            else return null;
        }

        /// <inheritdoc />
        public async Task<IHBaseScanner> OpenScanner(string tableName, HBaseScannerCreationOptions options = null)
        {
            EnsureNotDisposed();
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));

            var url = $"{_httpClient.BaseAddress}/{tableName}/scanner";

            var content = new StringContent(options.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.PutAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var id = (string)new JObject(result)["id"];// ???
                return new DefaultHBaseScanner(id, new HttpClient
                {
                    BaseAddress = new Uri($"{_httpClient.BaseAddress}/{tableName}/scanner/{id}")
                });
            }
            else return null;
        }

        /// <summary>
        /// 释放当前对象所占有的所有外部资源。
        /// </summary>
        /// <param name="disposing">用户是否正在调用 <see cref="Dispose()"/> 方法。</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
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
