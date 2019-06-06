using System;

namespace Stocker.Crawler.Tasks
{
    /// <summary>
    /// 封装一个 Crawler 任务的元数据。
    /// </summary>
    public sealed class CrawlerTaskMetadata
    {
        /// <summary>
        /// 初始化 <see cref="CrawlerTaskAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="type">Crawler 任务的实现类型。</param>
        /// <param name="annotation">在 Crawler 任务的实现类型上的 attribute 对象。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="type"/>为null
        ///     或
        ///     <paramref name="annotation"/>为null
        /// </exception>
        public CrawlerTaskMetadata(Type type, CrawlerTaskAttribute annotation)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Annotation = annotation ?? throw new ArgumentNullException(nameof(annotation));
        }
        
        /// <summary>
        /// 获取 Crawler 任务的实现类型。
        /// </summary>
        public Type Type { get; }
        
        /// <summary>
        /// 获取在 Crawler 任务实现类型上的 attribute 对象。
        /// </summary>
        public CrawlerTaskAttribute Annotation { get; }

        /// <summary>
        /// 激活 <see cref="Type"/> 类型的实例对象。
        /// </summary>
        /// <param name="serviceProvider">要使用的依赖注入容器。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/>为null</exception>
        /// <exception cref="InvalidOperationException">找不到能够满足所有参数依赖关系的构造器</exception>
        public ICrawlerTask ActivateTaskObject(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            foreach (var ctor in Type.GetConstructors())
            {
                var parameterInfos = ctor.GetParameters();
                var parameters = new object[parameterInfos.Length];

                var satisfied = true;    // All parameters satisfied?
                for (var i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        parameters[i] = serviceProvider.GetService(parameterInfos[i].ParameterType);
                    }
                    catch
                    {
                        if (parameterInfos[i].HasDefaultValue)
                        {
                            parameters[i] = parameterInfos[i].DefaultValue;
                        }
                        else
                        {
                            satisfied = false;
                            break;
                        }
                    }
                }

                if (!satisfied)
                {
                    continue;
                }

                return (ICrawlerTask) ctor.Invoke(parameters);
            }
            
            throw new InvalidOperationException($"没有找到能够满足类型{Type}依赖关系的构造器。");
        }
    }
}
