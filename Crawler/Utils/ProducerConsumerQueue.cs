using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Stocker.Crawler.Utils
{
    /// <summary>
    /// 封装消费者-生产者队列的实现。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ProducerConsumerQueue<T> : IDisposable
    {
        private readonly ConcurrentQueue<T> _q;
        private readonly SemaphoreSlim _sem;
        private bool _disposed;

        /// <summary>
        /// 初始化 <see cref="ProducerConsumerQueue{T}"/> 的新实例。
        /// </summary>
        /// <param name="capacity">队列的最大容量。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/>小于等于0</exception>
        public ProducerConsumerQueue(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _q = new ConcurrentQueue<T>();
            _sem = new SemaphoreSlim(0, capacity);

            _disposed = false;
        }

        /// <summary>
        /// 确保当前对象没有被 dispose。
        /// </summary>
        /// <exception cref="ObjectDisposedException">当当前对象已经被 dispose 时抛出</exception>
        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        /// 将给定的值加入队列。若队列已满，则该方法阻塞直到至少一个元素被移出队列为止。
        /// </summary>
        /// <param name="value">要添加的值。</param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Enqueue(T value)
        {
            EnsureNotDisposed();
            
            _q.Enqueue(value);
            _sem.Release();
        }

        /// <summary>
        /// 从队列中移除一个元素并返回移除的元素。若队列为空，则该方法阻塞知道至少一个元素被加入队列为止。
        /// </summary>
        /// <returns>被移除的值。</returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public T Dequeue()
        {
            EnsureNotDisposed();

            _sem.Wait();
            _q.TryDequeue(out var result);
            return result;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _sem.Dispose();
                _disposed = true;
            }
        }
    }
}
