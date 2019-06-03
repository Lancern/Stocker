using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stocker.HBase
{
    /// <summary>
    /// 提供对 HBase 中数据行的单元格集合的实现。
    /// </summary>
    public sealed class HBaseRowCellCollection : ICollection<HBaseCell>
    {
        /// <summary>
        /// 为 <see cref="HBaseCell"/> 提供比较器。
        /// </summary>
        private sealed class CellComparer : IComparer<HBaseCell>
        {
            public int Compare(HBaseCell x, HBaseCell y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return 1;
                }

                if (y == null)
                {
                    return -1;
                }

                return x.Timestamp - y.Timestamp;
            }
        }
        
        private readonly Dictionary<string, SortedSet<HBaseCell>> _cells;

        /// <summary>
        /// 初始化 <see cref="HBaseRowCellCollection"/> 类的新实例。
        /// </summary>
        public HBaseRowCellCollection()
        {
            _cells = new Dictionary<string, SortedSet<HBaseCell>>();
        }

        /// <inheritdoc />
        public int Count => _cells.Values.Sum(d => d.Count);

        /// <inheritdoc />
        public bool IsReadOnly => false;
        
        /// <inheritdoc />
        public IEnumerator<HBaseCell> GetEnumerator()
        {
            foreach (var cells in _cells.Values)
            {
                foreach (var cell in cells)
                {
                    yield return cell;
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(HBaseCell item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Column == null)
                throw new ArgumentException($"对象的{nameof(item.Column)}属性不能为null。");

            var column = item.Column.ToString();
            if (_cells.TryGetValue(column, out var cells))
            {
                cells.Add(item);
            }
            else
            {
                _cells.Add(column, new SortedSet<HBaseCell>(new CellComparer()) { item });
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            _cells.Clear();
        }

        /// <inheritdoc />
        public bool Contains(HBaseCell item)
        {
            var column = item.Column.ToString();
            if (!_cells.TryGetValue(column, out var cells))
            {
                return false;
            }

            return cells.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(HBaseCell[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            var space = array.Length - arrayIndex;
            if (space < Count)
                throw new ArgumentException("目标数组太小。");

            foreach (var cell in this)
            {
                array[arrayIndex++] = cell;
            }
        }

        /// <inheritdoc />
        public bool Remove(HBaseCell item)
        {
            var column = item.Column.ToString();
            if (!_cells.TryGetValue(column, out var cells))
            {
                return false;
            }

            return cells.Remove(item);
        }

        /// <summary>
        /// 查询位于给定列中的单元格。
        /// </summary>
        /// <param name="columnFamilyName">列族名称。</param>
        /// <param name="columnName">列名称。</param>
        /// <returns>包含在给定的列中的所有单元格。返回的单元格已经按照时间戳升序排序。</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="columnFamilyName"/>为null
        ///     或
        ///     <paramref name="columnName"/>为null
        /// </exception>
        public IEnumerable<HBaseCell> Get(string columnFamilyName, string columnName)
        {
            if (columnFamilyName == null)
                throw new ArgumentNullException(nameof(columnFamilyName));
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            var column = $"{columnFamilyName}:{columnName}";
            if (!_cells.TryGetValue(column, out var cells))
            {
                return new HBaseCell[0];
            }

            return cells;
        }
    }
}
