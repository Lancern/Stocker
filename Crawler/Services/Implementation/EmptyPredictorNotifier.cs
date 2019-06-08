using System.Threading.Tasks;

namespace Stocker.Crawler.Services.Implementation
{
    /// <summary>
    /// 提供 <see cref="IPredictorNotifier"/> 的空实现。
    /// </summary>
    internal sealed class EmptyPredictorNotifier : IPredictorNotifier
    {
        /// <inheritdoc />
        public Task Notify()
        {
            return Task.CompletedTask;
        }
    }
}
