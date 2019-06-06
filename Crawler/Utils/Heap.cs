using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocker.Crawler.Utils
{
    /// <summary>
    /// 提供大根堆的实现。
    /// </summary>
    /// <typeparam name="T">集合内元素的类型。</typeparam>
    public sealed class Heap<T>
    {
        private const int RootIndex = 1;
        
        private readonly List<T> _array;    // _array[0] will not be used and will be assigned a placeholder.
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// 初始化 <see cref="Heap{T}"/> 的新实例。
        /// </summary>
        /// <param name="elements">堆中初始的元素。</param>
        /// <param name="comparer">要使用的元素比较器。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="elements"/>为null
        ///     或
        ///     <paramref name="comparer"/>为null
        /// </exception>
        public Heap(IEnumerable<T> elements, IComparer<T> comparer)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            _array = new List<T>(new[] { default(T) /* Placeholder for _array[0] */ }.Concat(elements));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            
            MakeHeap();
        }

        /// <summary>
        /// 初始化 <see cref="Heap{T}"/> 的新实例。
        /// </summary>
        /// <param name="comparer">要使用的元素比较器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/>为null</exception>
        public Heap(IComparer<T> comparer) : this(new T[0], comparer)
        {
        }

        /// <summary>
        /// 初始化 <see cref="Heap{T}"/> 的新实例。
        /// </summary>
        public Heap() : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// 交换 <see cref="_array"/> 中位于给定两个位置处的元素。
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private void Swap(int index1, int index2)
        {
            var tmp = _array[index1];
            _array[index1] = _array[index2];
            _array[index2] = tmp;
        }

        /// <summary>
        /// 从给定的堆内位置开始，向根方向调整大根堆。
        /// </summary>
        /// <param name="index"></param>
        private void ShiftUp(int index)
        {
            while (index != RootIndex)
            {
                var parent = index / 2;
                if (_comparer.Compare(_array[parent], _array[index]) >= 0)
                {
                    break;
                }
                
                Swap(parent, index);
                index = parent;
            }
        }

        /// <summary>
        /// 从给定的堆内位置开始，向叶方向调整大根堆。
        /// </summary>
        /// <param name="index"></param>
        private void ShiftDown(int index)
        {
            while (index < _array.Count)
            {
                var leftChild = index * 2;
                var rightChild = leftChild + 1;

                if (leftChild >= _array.Count)
                {
                    break;
                }

                var d = 0; // 0: swap with left child; 1: swap with right child.
                if (rightChild < _array.Count && _comparer.Compare(_array[leftChild], _array[rightChild]) < 0)
                {
                    d = 1;
                }

                var targetChild = index * 2 + d;
                if (_comparer.Compare(_array[index], _array[targetChild]) >= 0)
                {
                    break;
                }

                Swap(targetChild, index);
                index = targetChild;
            }
        }

        /// <summary>
        /// 在线性复杂度内将 <see cref="_array"/> 内的元素调整为大根堆顺序。
        /// </summary>
        private void MakeHeap()
        {
            for (var i = 2; i < _array.Count; i++)
            {
                ShiftUp(i);
            }
        }

        /// <summary>
        /// 获取堆中包含的元素数量。
        /// </summary>
        public int Count => _array.Count - 1;

        /// <summary>
        /// 获取堆中的最大元素。
        /// </summary>
        /// <exception cref="InvalidOperationException">当堆为空时。</exception>
        public T MaxElement
        {
            get
            {
                if (Count == 0)
                    throw new InvalidOperationException("尝试获取空堆的最大元素。");
                return _array[RootIndex];
            }
        }

        /// <summary>
        /// 向大根堆添加元素。
        /// </summary>
        /// <param name="value">要添加的元素。</param>
        public void Add(T value)
        {
            _array.Add(value);
            ShiftUp(_array.Count - 1);
        }

        /// <summary>
        /// 移除大根堆中最大的元素。
        /// </summary>
        /// <exception cref="InvalidOperationException">当堆为空时。</exception>
        public T RemoveMax()
        {
            if (Count == 0)
                throw new InvalidOperationException("尝试删除空堆的最大元素。");

            var removedElement = _array[RootIndex];
            
            Swap(RootIndex, _array.Count - 1);
            _array.RemoveAt(_array.Count - 1);
            ShiftDown(RootIndex);

            return removedElement;
        }
    }
}
