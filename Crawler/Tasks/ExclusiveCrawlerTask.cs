using System.Threading.Tasks;
using Stocker.Crawler.Utils;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 为互斥运行的 Crawler Task 提供抽象基类。
    /// </summary>
    public abstract class ExclusiveCrawlerTask : ICrawlerTask
    {
        private int _exclusiveGuard;

        /// <summary>
        /// 初始化 <see cref="ExclusiveCrawlerTask"/> 类的新实例。
        /// </summary>
        protected ExclusiveCrawlerTask()
        {
            ExclusiveRegionGuard.InitializeGuard(out _exclusiveGuard);
        }
        
        /// <summary>
        /// 当在子类中重写时，在互斥环境中运行 Crawler Task。
        /// </summary>
        /// <returns></returns>
        protected abstract Task RunExclusive();
        
        /// <inheritdoc />
        public async Task Run()
        {
            if (!ExclusiveRegionGuard.TryEnter(ref _exclusiveGuard))
            {
                return;
            }

            try
            {
                await RunExclusive();
            }
            finally
            {
                ExclusiveRegionGuard.Leave(ref _exclusiveGuard);
            }
        }
    }
}
