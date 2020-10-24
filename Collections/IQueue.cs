using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Queue data structure of type T.
    /// Follows FIFO protocol.
    /// </summary>
    public interface IQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Places an object onto the end of the queue.
        /// </summary>
        /// <param name="payload">Object to place</param>
        void Enqueue(T payload);

        /// <summary>
        /// Removes an object from the front of the queue;
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        /// <returns>Object removed</returns>
        T Dequeue();

        /// <summary>
        /// Returns an object from the front of the queue;
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        T Peek();

        /// <summary>
        /// Alias for Enqueue.
        /// </summary>
        void Push(T payload);

        /// <summary>
        /// Alias for Dequeue.
        /// </summary>
        /// <returns></returns>
        T Pop();

        /// <summary>
        /// Returns the number of objects in the queue.
        /// </summary>
        int Size();

        bool IsEmpty();
    }
}
