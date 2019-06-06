using Microsoft.Extensions.Logging;

namespace Stocker.Crawler.Services.Implementation
{
    /// <summary>
    /// 提供 <see cref="DefaultApplicationRunner"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultApplicationRunner : IApplicationRunner
    {
        private readonly ILogger<DefaultApplicationRunner> _logger;
        
        public DefaultApplicationRunner(ILogger<DefaultApplicationRunner> logger)
        {
            _logger = logger;
        }
        
        /// <inheritdoc />
        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}
