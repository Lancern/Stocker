using System.Threading.Tasks;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 为一个定时后台任务提供抽象。
    /// </summary>
    public interface ICrawlerTask
    {
        /// <summary>
        /// 运行任务。
        /// </summary>
        /// <returns></returns>
        Task Run();
    }
}
