using System;

namespace Collections
{
    /// <summary>
    /// Priority queue data structure.
    /// Returns the smallest item in the collection instead of following the FIFO protocol. 
    /// </summary>
    public interface IPriorityQueue<K> where K : IComparable<K>
    {
        /// <summary>
        /// Adds a key into the priority queue.
        /// </summary>
        void Push(K key);

        /// <summary>
        /// Returns the smallest key in the priority queue.
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        K Peek();

        /// <summary>
        /// Removes and returns the smallest key in the priority queue.
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        K Pop();

        /// <summary>
        /// Returns the number of keys in the priority queue.
        /// </summary>
        int Size();

        bool IsEmpty();
    }

    public interface IPriorityQueue<K, V> where K : IComparable<K>
    {
        bool Contains(V value);

        void Push(V value, K key);

        void Update(V value, K key);

        (K key, V value) Peek();

        (K key, V value) Pop();

        int Size();

        bool IsEmpty();
    }
}
