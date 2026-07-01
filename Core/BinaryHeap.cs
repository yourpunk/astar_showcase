using System;
using System.Collections.Generic;

namespace ASTARShowcase.Core
{
    public class BinaryHeap<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly Comparison<T> _compare;
        public int Count => _items.Count;
        public BinaryHeap(Comparison<T> compare)
        {
            _compare = compare ?? throw new ArgumentNullException(nameof(compare));
        }

        public bool Contains(T item) => _indexOf.ContainsKey(item);
        public void Insert(T item)
        {
            _items.Add(item);
            int i = _items.Count - 1;
            _indexOf[item] = i;
            SiftUp(i);
        }
        public T ExtractMin()
        {
            if (_items.Count == 0) throw new InvalidOperationException("Heap is empty.");
            T min = _items[0];
            int lastIndex = _items.Count - 1;
            Swap(0, lastIndex);
            _items.RemoveAt(lastIndex);
            _indexOf.Remove(min);
            if (_items.Count > 0) SiftDown(0);
            return min;
        }
        public void UpdatePriority(T item)
        {
            if (!_indexOf.TryGetValue(item, out int i)) throw new InvalidOperationException("Item is not in the heap.");
            SiftUp(i);
            SiftDown(i);
        }

        public void Clear()
        {
            _items.Clear();
            _indexOf.Clear();

        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_compare(_items[i], _items[parent]) >= 0) break;
                Swap(i, parent);
                i = parent;
            }
        }
        private void SiftDown(int i)
        {
            int count = _items.Count;
            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int smallest = i;
                if (left < count &&  _compare(_items[left], _items[smallest]) < 0) smallest = left;
                if (right < count &&  _compare(_items[right], _items[smallest]) < 0) smallest = right;
                if (smallest == i) break;
                Swap(i, smallest);
                i = smallest;
            }
        }
        private void Swap(int i, int j)
        {
            T temp = _items[i];
            _items[i] = _items[j];
            _items[j] = temp;
            _indexOf[_items[i]] = i;
            _indexOf[_items[j]] = j;
        }
    }
}