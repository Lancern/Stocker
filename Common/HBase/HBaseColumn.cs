using System;

namespace Stocker.HBase
{
    /// <summary>
    /// 表示 HBase 的列。
    /// </summary>
    public sealed class HBaseColumn
    {
        /// <summary>
        /// 初始化 <see cref="HBaseColumn"/> 类的新实例。
        /// </summary>
        /// <param name="columnFamilyName">列族名称。</param>
        /// <param name="columnName">列名称。</param>
        public HBaseColumn(string columnFamilyName, string columnName)
        {
            ColumnFamilyName = columnFamilyName ?? throw new ArgumentNullException(nameof(columnFamilyName));
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        }
        
        /// <summary>
        /// 获取列族名称。
        /// </summary>
        public string ColumnFamilyName { get; }
        
        /// <summary>
        /// 获取列名称。
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 获取当前对象的哈希值。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// 测试当前对象是否与给定的对象相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is HBaseColumn))
            {
                return false;
            }

            return ToString() == obj.ToString();
        }

        /// <summary>
        /// 获取当前对象的字符串表示。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{ColumnFamilyName}:{ColumnName}";
        }

        public static bool operator ==(HBaseColumn col1, HBaseColumn col2)
        {
            if (ReferenceEquals(col1, null))
            {
                return ReferenceEquals(col2, null);
            }

            return col1.Equals(col2);
        }

        public static bool operator !=(HBaseColumn col1, HBaseColumn col2)
        {
            return col1 != col2;
        }
    }
}
