using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// List data structure of type <c>T</c>.
    /// Follows LIFO protocol.
    /// </summary>
    public interface IList<T> : IEnumerable<T>
    {
        /// <summary>
        /// Returns the head of the list;
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        T Peek();

        bool IsEmpty();

        /// <summary>
        /// Removes and returns the head of the list;
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        T Pop();

        /// <summary>
        /// Places the payload onto the head of the list.
        /// </summary>
        void Push(T payload);

        /// <summary>
        /// Returns the number of elements in the list;
        /// </summary>
        int Size();
    }
}
