using System;

namespace Collections
{
    /// <summary>
    /// Min-oriented Priority Queue using a Binary Heap with dynamically-resizing backing array.
    /// </summary>
    public class BinaryHeap<K> : IPriorityQueue<K> where K : class, IComparable<K>
    {
        private K[] heap;
        private int n = 0;  // The number of keys on the heap

        public BinaryHeap(int capacity = 10)
        {
            // A (minimum) heap-ordered complete binary tree is a binary tree which is completely balanced, 
            // except possibly for the lowest levels, and also one  where the parent node's key is
            // at most as large as that of its children. In the array representation:
            // - Parent of k is given by k/2.
            // - Children of k are 2*k, 2*k+1
            // - The heap invariant states heap[k] <= Min(heap[2*k], heap[2*k+1])
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            heap = new K[capacity + 1];
        }

        public bool IsHeapOrdered()
        {
            // There are plenty of ways to write this, but the code from Algs. I 
            // refects the symmetry of the recursive solution
            bool IsHeapOrdered(int k)
            {
                if (k > n)
                    return true;
                var left = 2 * k;
                var right = 2 * k + 1;
                if (left <= n && Greater(k, left))
                    return false;
                if (right <= n && Greater(k, right))
                    return false;
                return IsHeapOrdered(left) && IsHeapOrdered(right);
            }
            return IsHeapOrdered(1);
        }

        public bool IsEmpty() => n == 0;

        public void Push(K key)
        {
            // Lazily increase the size of the backing array if necessary
            if (n == heap.Length - 1)
            {
                Resize(2 * heap.Length);
            }
            heap[++n] = key;
            Swim(n);
        }

        public K Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty heap");
            }
            return heap[1];
        }

        public K Pop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty heap");
            }
            // Lazily reduce the size of the backing array if necessary
            if (n == heap.Length / 4)
            {
                Resize(heap.Length / 2);
            }

            var smallest = heap[1];
            Swap(1, n);
            heap[n--] = null;  // No loitering
            Sink(1);
            return smallest;
        }

        public int Size() => n;

        public int Capacity() => heap.Length;

        private void Resize(int capacity)
        {
            var temp = new K[capacity];
            for (int i = 1; i <= n; i++)
            {
                temp[i] = heap[i];
            }
            heap = temp;
        }

        // Utilities to restore the heap invariant

        private void Sink(int k)
        {
            while (2 * k <= n)
            {
                int samllest = 2 * k < n && Greater(2 * k, 2 * k + 1) ? 2 * k + 1 : 2 * k;
                if (!Greater(k, samllest)) break;
                Swap(k, samllest);
                k = samllest;
            }
        }

        private void Swim(int k)
        {
            while (k > 1 && Greater(k / 2, k))
            {
                Swap(k / 2, k);
                k = k / 2;
            }
        }

        private bool Greater(int i, int j) => heap[i].CompareTo(heap[j]) > 0;

        private void Swap(int i, int j)
        {
            (heap[i], heap[j]) = (heap[j], heap[i]);
        }
    }

    public class BinaryHeap<K, V> : IPriorityQueue<K, V> where K : class, IComparable<K> where V : class
    {
        private K[] keys;
        private V[] values;
        private HashMap<V, int> index;
        private int n = 0;  // The number of keys on the heap

        public BinaryHeap(int capacity = 10)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            keys = new K[capacity + 1];
            values = new V[capacity + 1];
            index = new HashMap<V, int>();
        }

        public bool IsHeapOrdered()
        {
            bool IsHeapOrdered(int k)
            {
                if (k > n) return true;
                var left = 2 * k;
                var right = 2 * k + 1;
                if (left <= n && Greater(k, left)) return false;
                if (right <= n && Greater(k, right)) return false;
                return IsHeapOrdered(left) && IsHeapOrdered(right);
            }
            return IsHeapOrdered(1);
        }

        public bool IsEmpty() => n == 0;

        public bool Contains(V value) => index.Contains(value);

        public void Push(V value, K key)
        {
            if (index.Contains(value))
            {
                throw new InvalidOperationException();  // Values must be unique
            }

            // Lazily increase the size of the backing array if necessary
            if (n == keys.Length - 1)
            {
                Resize(2 * keys.Length);
            }
            n++;
            keys[n] = key;
            values[n] = value;
            index[value] = n;  // Initially give this pair an the heapsize as an index
            Swim(n);
        }

        public void Update(V value, K key)
        {
            var i = index[value];
            var prev = keys[i];
            keys[i] = key;

            if (Greater(key, prev))
            {
                Sink(i);
            }
            else
            {
                Swim(i);
            }
        }

        public (K key, V value) Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty heap");
            }
            return (keys[1], values[1]);
        }

        public (K key, V value) Pop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty heap");
            }
            // Lazily reduce the size of the backing array if necessary
            if (n == keys.Length / 4)
            {
                Resize(keys.Length / 2);
            }

            var (key, value) = (keys[1], values[1]);
            Swap(1, n);
            n--;
            (keys[n], values[n]) = (null, null);  // No loitering
            Sink(1);
            index.Delete(value);  // Clear index

            return (key, value);
        }

        public int Size() => n;

        public int Capacity() => keys.Length;

        private void Resize(int capacity)
        {
            var newKeys = new K[capacity];
            var newValues = new V[capacity];
            for (int i = 1; i <= n; i++)
            {
                newKeys[i] = keys[i];
                newValues[i] = values[i];
            }
            keys = newKeys;
            values = newValues;
        }

        // Utilities to restore the heap invariant

        private void Sink(int k)
        {
            while (2 * k <= n)
            {
                int samllest = 2 * k < n && Greater(2 * k, 2 * k + 1) ? 2 * k + 1 : 2 * k;
                if (!Greater(k, samllest)) break;
                Swap(k, samllest);
                k = samllest;
            }
        }

        private void Swim(int k)
        {
            while (k > 1 && Greater(k / 2, k))
            {
                Swap(k / 2, k);
                k = k / 2;
            }
        }

        private bool Greater(int i, int j) => keys[i].CompareTo(keys[j]) > 0;

        private bool Greater(K k, K l) => k.CompareTo(l) > 0;

        private void Swap(int i, int j)
        {
            (keys[i], keys[j]) = (keys[j], keys[i]);
            (index[values[i]], index[values[j]]) = (j, i);  // Maintain the index during a swap
            (values[i], values[j]) = (values[j], values[i]);
        }
    }
}
