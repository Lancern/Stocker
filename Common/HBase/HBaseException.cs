using System;
using System.Runtime.Serialization;

namespace Stocker.HBase
{
    /// <summary>
    /// 为 HBase 提供异常基类。
    /// </summary>
    [Serializable]
    public class HBaseException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// 初始化 <see cref="HBaseException"/> 类的新实例。
        /// </summary>
        public HBaseException()
        {
        }

        /// <summary>
        /// 初始化 <see cref="HBaseException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public HBaseException(string message) : base(message)
        {
        }

        /// <summary>
        /// 初始化 <see cref="HBaseException"/> 类的新实例。
        /// </summary>
        /// <exception cref="message">异常消息。</exception>
        /// <exception cref="inner">引发当前异常的内部异常。</exception>
        public HBaseException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// 从序列化环境中反序列化 <see cref="HBaseException"/> 的实例对象。
        /// </summary>
        protected HBaseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
