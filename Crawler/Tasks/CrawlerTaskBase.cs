using System.Threading.Tasks;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 为一个定时后台任务提供抽象。
    /// </summary>
    public abstract class CrawlerTaskBase
    {
        /// <summary>
        /// 当在子类中重写时，对当前后台任务及其依赖项进行初始化。
        /// </summary>
        public virtual void Initialize()
        {
        }
        
        /// <summary>
        /// 运行任务。
        /// </summary>
        /// <returns></returns>
        public abstract Task Run();
    }
}
