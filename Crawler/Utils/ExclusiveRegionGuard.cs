using System.Threading;

namespace Stocker.Crawler.Utils
{
    /// <summary>
    /// 提供互斥区间的实现。
    /// </summary>
    public static class ExclusiveRegionGuard
    {
        /// <summary>
        /// 初始化互斥区间的锁变量。
        /// </summary>
        /// <param name="guard"></param>
        public static void InitializeGuard(out int guard)
        {
            guard = 0;
        }
        
        /// <summary>
        /// 尝试进入互斥区间。
        /// </summary>
        /// <param name="guard">互斥区间的锁变量。</param>
        /// <returns>是否成功进入互斥区间。</returns>
        public static bool TryEnter(ref int guard)
        {
            return Interlocked.Exchange(ref guard, 1) == 0;
        }

        /// <summary>
        /// 退出互斥区间。
        /// </summary>
        /// <param name="guard">互斥区间的锁变量。</param>
        public static void Leave(ref int guard)
        {
            Interlocked.Exchange(ref guard, 0);
        }
    }
}
