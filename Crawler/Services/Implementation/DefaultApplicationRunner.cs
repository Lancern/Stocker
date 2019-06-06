using Microsoft.Extensions.Logging;
using Stocker.Crawler.Tasks;

namespace Stocker.Crawler.Services.Implementation
{
    /// <summary>
    /// 提供 <see cref="DefaultApplicationRunner"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultApplicationRunner : IApplicationRunner
    {
        private readonly ICrawlerTaskManagerFactory _crawlerTaskManagerFactory;
        private readonly ILogger<DefaultApplicationRunner> _logger;
        
        /// <summary>
        /// 初始化 <see cref="DefaultApplicationRunner"/> 类的新实例。
        /// </summary>
        public DefaultApplicationRunner(ICrawlerTaskManagerFactory crawlerTaskManagerFactory, 
                                        ILogger<DefaultApplicationRunner> logger)
        {
            _crawlerTaskManagerFactory = crawlerTaskManagerFactory;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public void Run()
        {
            _crawlerTaskManagerFactory.Create().Run();
        }
    }
}
