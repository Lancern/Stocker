using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Stocker.Crawler.Tasks.Implementation
{
    /// <summary>
    /// 提供 <see cref="ICrawlerTaskManagerFactory"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultCrawlerTaskManagerFactory : ICrawlerTaskManagerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DefaultCrawlerTaskManager> _logger;

        /// <summary>
        /// 初始化 <see cref="DefaultCrawlerTaskManagerFactory"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider">依赖注入容器的 <see cref="IServiceProvider"/> 切面</param>
        /// <param name="logger">日志支撑件</param>
        public DefaultCrawlerTaskManagerFactory(IServiceProvider serviceProvider,
                                                ILogger<DefaultCrawlerTaskManager> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public ICrawlerTaskManager Create()
        {
            var crawlerTaskMetadatas = new List<CrawlerTaskMetadata>();
            
            // 遍历所有已经加载的 CLR 类型
            _logger.LogInformation("探查后台任务类型...");
            foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in loadedAssembly.GetTypes())
                {
                    var annotation = t.GetCustomAttribute<CrawlerTaskAttribute>();
                    if (annotation == null)
                    {
                        continue;
                    }

                    if (t.GetInterface(nameof(ICrawlerTask)) == null)
                    {
                        continue;
                    }
                    
                    // t 带有 CrawlerTaskAttribute 注解且实现了 ICrawlerTask
                    _logger.LogInformation("找到后台任务类型：{0}，启动间隔为 {1} 分钟", t, annotation.Interval);
                    var metadata = new CrawlerTaskMetadata(t, annotation);
                    crawlerTaskMetadatas.Add(metadata);
                }
            }
            
            return new DefaultCrawlerTaskManager(crawlerTaskMetadatas, _serviceProvider, _logger);
        }
    }
}
