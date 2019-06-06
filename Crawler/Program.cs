using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Stocker.Crawler.Services.DependencyInjection;
using Stocker.Crawler.Services;
using Stocker.Crawler.Tasks.DependencyInjection;
using Stocker.HBase.DependencyInjection;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Stocker.Crawler
{
    /// <summary>
    /// 为应用程序提供入口点。
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 初始化应用程序所需的依赖服务。
        /// </summary>
        /// <param name="services">依赖服务集。</param>
        /// <param name="config">应用程序配置信息。</param>
        /// <param name="logger">日志支撑件。</param>
        /// <returns>服务容器初始化是否成功完成。</returns>
        private static bool InitializeServiceCollection(IServiceCollection services, IConfiguration config, Logger logger)
        {
            // 添加配置服务
            services.AddSingleton(config);
            
            // 添加 NLog 日志依赖服务
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog();
            });
            
            // 添加 HBase Client Factory
            services.AddHBaseClientFactory();
            
            // 添加 IHttpClientFactory 依赖
            services.AddHttpClient();
            
            // 添加应用程序业务入口
            services.AddApplicationRunner();
            
            // 添加 Crawler Task Factory
            services.AddDefaultCrawlerTaskManagerFactory();
            
            // 添加数据爬虫
            var stockListAppId = config.GetSection("Shenjian").GetValue<string>("StockListAppId");
            var stockDailyStatAppId = config.GetSection("Shenjian").GetValue<string>("StockDailyStatAppId");
            if (string.IsNullOrEmpty(stockListAppId) || string.IsNullOrEmpty(stockDailyStatAppId))
            {
                logger.Error("没有配置访问神箭数据所需的 App ID。");
                return false;
            }
            services.AddDefaultCrawler(stockListAppId, stockDailyStatAppId);

            return true;
        }
        
        /// <summary>
        /// 应用程序入口点。
        /// </summary>
        /// <param name="args">应用程序参数</param>
        public static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                // 读取应用程序配置
                var config = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                             .AddJsonFile("secretsettings.json", optional: true, reloadOnChange: false)
                             .Build();

                // 初始化依赖注入容器
                var services = new ServiceCollection();
                if (!InitializeServiceCollection(services, config, logger))
                {
                    logger.Fatal("服务容器初始化失败。");
                    return;
                }

                using (var serviceProvider = services.BuildServiceProvider())
                {
                    serviceProvider.GetService<IApplicationRunner>().Run();
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "应用程序抛出了未经处理的异常：{0}", ex);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
