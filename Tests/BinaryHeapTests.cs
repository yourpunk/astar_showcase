using System;
using System.Collections.Generic;
using AStarShowcase.Core;
using NUnit.Framework;

namespace AStarShowcase.Tests
{
    public class BinaryHeapTests
    {
        private static BinaryHeap<int> CreateIntHeap()
        {
            return new BinaryHeap<int>((a,b) => a.CompareTo(b));
        }
        [Test]
        public void ExtractMin_ReturnsItemsInAscendingOrder()
        {
            var heap = CreateIntHeap();
            int[] values = {5,1,9,3,7,2,9,4,6};
            foreach (var v in values) heap.Insert(v);
            var extracted = new List<int>();
            while (heap.Count > 0) extracted.Add(heap.ExtractMin());
            var expected = new List<int>(values);
            expected.Sort();
            CollectionAssert.AreEqual(expected, extracted);
        }
        [Test]
        public void Count_ReflectsNumberOfInsertedItems()
        {
            var heap = CreateIntHeap();
            Assert.AreEqual(0, heap.Count);
 
            heap.Insert(2);
            heap.Insert(3);
            Assert.AreEqual(2, heap.Count);
            heap.ExtractMin();
            Assert.AreEqual(1, heap.Count);
        }
        [Test]
        public void ExtractMin_OnEmptyHeap_Throws()
        {
            var heap = CreateIntHeap();
            Assert.Throws<InvalidOperationException>(() => heap.ExtractMin());
            
        }
        [Test]
        public void Contains_ReflectsCurrentMembership()
        {
            var heap = CreateIntHeap();
            heap.Insert(42);
            Assert.IsTrue(heap.Contains(42));
            Assert.IsFalse(heap.Contains(7));
            heap.ExtractMin();
            Assert.IsFalse(heap.Contains(42));
            
        }
        [Test]
        public void UpdatePriority_AfterPriorityDecreases_ItemExtractedEarlier()
        {
            var priorities = new Dictionary<string, float>
            {
                ["A"] = 10f,
                ["B"] = 5f,
                ["C"] = 8f
            };
            var heap = new BinaryHeap<string>((a,b) => priorities[a].CompareTo(priorities[b]));
            heap.Insert("A");
            heap.Insert("B");
            heap.Insert("C");
            priorities["A"] = 1f;
            heap.UpdatePriority("A");
            Assert.AreEqual("A", heap.ExtractMin());
            Assert.AreEqual("B", heap.ExtractMin());
            Assert.AreEqual("C", heap.ExtractMin());          
        }
        [Test]
        public void UpdatePriority_ItemNotInHeap_Throws()
        {
            var heap = CreateIntHeap();
            heap.Insert(1);
            Assert.Throws<InvalidOperationException>(() => heap.UpdatePriority(999));
        }
        [Test]
        public void Clear_RemovesAllItems()
        {
            var heap = CreateIntHeap();
            heap.Insert(1);
            heap.Insert(2);
            heap.Clear();
            Assert.AreEqual(0, heap.Count);
            Assert.IsFalse(heap.Contains(1));            
        }
        [Test] 
        public void Insert_DuplicateValues_ExtractOrderStillCorrect()
        {
            var heap = CreateIntHeap();
            heap.Insert(3);
            heap.Insert(3);
            heap.Insert(1);
            Assert.AreEqual(1, heap.ExtractMin());
            Assert.AreEqual(3, heap.ExtractMin());
            Assert.AreEqual(3, heap.ExtractMin());           
        }

    }
}