using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 为 Crawler 任务提供管理。
    /// </summary>
    public interface ICrawlerTaskManager
    {
        /// <summary>
        /// 运行 Crawler 任务计划。
        /// </summary>
        /// <returns></returns>
        void Run();
    }
}
