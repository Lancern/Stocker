using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stocker.HBase.Implementations
{
    /// <summary>
    /// 提供 <see cref="IHBaseScanner"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultHBaseScanner : IHBaseScanner
    {
        private readonly string _id;
        private readonly HttpClient _httpClient;
        private bool _exhausted;
        private bool _disposed;

        /// <summary>
        /// 初始化 <see cref="DefaultHBaseScanner"/> 的新实例。
        /// </summary>
        /// <param name="id">Scanner 的 ID。</param>
        /// <param name="client">用于发送和接收 HTTP 请求的 <see cref="HttpClient"/> 对象。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="id"/>为null
        ///     或
        ///     <paramref name="client"/>为null
        /// </exception>
        public DefaultHBaseScanner(string id, HttpClient client)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _httpClient = client ?? throw new ArgumentNullException(nameof(client));
            _exhausted = false;
            _disposed = false;
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
        public string Id
        {
            get
            {
                EnsureNotDisposed();
                return _id;
            }
        }

        /// <inheritdoc />
        public bool HasExhausted
        {
            get
            {
                EnsureNotDisposed();
                return _exhausted;
            }
        }
        
        /// <inheritdoc />
        public async Task<List<HBaseRow>> ReadNextBatch()
        {
            EnsureNotDisposed();

            var response = await _httpClient.GetAsync(string.Empty);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync();
                var rows = new List<HBaseRow>();
                var json = new JObject(result);

                // TODO: 将返回的内容转化为HBaseRow列表

                return rows;
            }
            else return null;
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
                    _httpClient.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
