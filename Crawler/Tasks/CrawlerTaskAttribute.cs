using System;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 标识 Crawler 任务的实现类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CrawlerTaskAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="CrawlerTaskAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="interval">任务的运行间隔，单位为分钟。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/>小于等于0</exception>
        public CrawlerTaskAttribute(int interval)
        {
            if (interval <= 0)
                throw new ArgumentOutOfRangeException(nameof(interval));
            
            Interval = interval;
        }
        
        /// <summary>
        /// 获取任务的运行间隔，单位为分钟。
        /// </summary>
        public int Interval { get; }
    }
}
