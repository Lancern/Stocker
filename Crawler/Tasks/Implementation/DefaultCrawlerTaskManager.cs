using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stocker.Crawler.Utils;

namespace Stocker.Crawler.Tasks.Implementation
{
    /// <summary>
    /// 提供 <see cref="ICrawlerTaskManager"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultCrawlerTaskManager : ICrawlerTaskManager
    {
        private sealed class CrawlerTaskScheduleDescriptor
        {
            public sealed class HeapComparer : Comparer<CrawlerTaskScheduleDescriptor>
            {
                public override int Compare(CrawlerTaskScheduleDescriptor x, 
                                            CrawlerTaskScheduleDescriptor y)
                {
                    if (x == null && y == null)
                    {
                        return 0;
                    }

                    if (x == null)
                    {
                        return -1;
                    }

                    if (y == null)
                    {
                        return 1;
                    }

                    return -(x.NextRunTimestamp - y.NextRunTimestamp);
                }
            }

            public CrawlerTaskScheduleDescriptor(int nextRunTimestamp, CrawlerTaskMetadata metadata)
            {
                NextRunTimestamp = nextRunTimestamp;
                Metadata = metadata;
            }
            
            public int NextRunTimestamp { get; set; }
            
            public CrawlerTaskMetadata Metadata { get; }
        }

        private readonly Heap<CrawlerTaskScheduleDescriptor> _tasks;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DefaultCrawlerTaskManager> _logger;

        public DefaultCrawlerTaskManager(IEnumerable<CrawlerTaskMetadata> metadatas,
                                         IServiceProvider serviceProvider,
                                         ILogger<DefaultCrawlerTaskManager> logger)
        {
            if (metadatas == null)
                throw new ArgumentNullException(nameof(metadatas));

            var descriptors = metadatas.Select(
                meta => new CrawlerTaskScheduleDescriptor(meta.Annotation.Interval, meta));

            _tasks = new Heap<CrawlerTaskScheduleDescriptor>(
                descriptors, new CrawlerTaskScheduleDescriptor.HeapComparer());

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <inheritdoc />
        public void Run()
        {
            if (_tasks.Count == 0)
            {
                _logger.LogWarning("没有任何的 Crawler 任务。");
                return;
            }

            var ts = 0;    // Current timestamp
            var crawlerTaskId = 0;
            while (true)
            {
                var interval = _tasks.MaxElement.NextRunTimestamp - ts;
                if (interval > 0)
                {
                    Thread.Sleep(interval * 60 * 1000);
                }

                if (interval >= 0)
                {
                    ts += interval;
                }
                
                while (_tasks.MaxElement.NextRunTimestamp == ts)
                {
                    var activeTask = _tasks.RemoveMax();

                    CrawlerTaskBase taskObject = null;
                    try
                    {
                        taskObject = activeTask.Metadata.ActivateTaskObject(_serviceProvider);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "无法激活 Crawler 任务对象 {0}：{1}：{2}",
                                         activeTask.Metadata.Type, ex.GetType(), ex.Message);
                    }

                    if (taskObject != null)
                    {
                        var taskId = ++crawlerTaskId;
                        _logger.LogInformation("启动 Crawler 任务 {0}，ID = {1}", activeTask.Metadata.Type, taskId);
                        Task.Run(taskObject.Run)
                            .ContinueWith(t => _logger.LogInformation("Crawler 任务 {0} 已退出", taskId));
                    }
                    
                    // activeTask.Metadata.Annotation.Interval should be guaranteed to be greater than 0.

                    activeTask.NextRunTimestamp += activeTask.Metadata.Annotation.Interval;
                    _tasks.Add(activeTask);
                }
            }
        }
    }
}
