// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/collector/top_score_collector.rs (TopNComputer) and Apache Lucene
// lucene/core/src/java/org/apache/lucene/util/PriorityQueue.java with HitQueue.java. Translated from Rust and
// Java to C#: a bounded binary heap whose root is the worst kept item.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Scoring
{
    /// <summary>Keeps the best <c>k</c> items under a comparer where a negative result means "better".</summary>
    public sealed class TopK<T>
    {
        private readonly int _k;
        private readonly Comparison<T> _better;
        private readonly List<T> _heap;

        public TopK(int k, Comparison<T> better)
        {
            if (k < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(k));
            }
            if (better is null)
            {
                throw new ArgumentNullException(nameof(better));
            }
            _k = k;
            _better = better;
            _heap = new List<T>(Math.Min(k, 1024));
        }

        public int Count => _heap.Count;

        /// <summary>Offers an item; kept when the heap is not full or it beats the worst kept item.</summary>
        public void Add(T item)
        {
            if (_heap.Count < _k)
            {
                _heap.Add(item);
                SiftUp(_heap.Count - 1);
                return;
            }
            if (_better(item, _heap[0]) < 0)
            {
                _heap[0] = item;
                SiftDown(0);
            }
        }

        /// <summary>The kept items, best first.</summary>
        public List<T> ToSortedList()
        {
            List<T> list = new List<T>(_heap);
            list.Sort(_better);
            return list;
        }

        // Heap order: parent is WORSE than (or equal to) children, so the root is the worst.
        private bool Worse(int a, int b) => _better(_heap[a], _heap[b]) > 0;

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (!Worse(i, parent))
                {
                    break;
                }
                Swap(i, parent);
                i = parent;
            }
        }

        private void SiftDown(int i)
        {
            int n = _heap.Count;
            while (true)
            {
                int left = 2 * i + 1;
                int right = left + 1;
                int worst = i;
                if (left < n && Worse(left, worst)) { worst = left; }
                if (right < n && Worse(right, worst)) { worst = right; }
                if (worst == i)
                {
                    return;
                }
                Swap(i, worst);
                i = worst;
            }
        }

        private void Swap(int a, int b)
        {
            T tmp = _heap[a];
            _heap[a] = _heap[b];
            _heap[b] = tmp;
        }
    }
}
