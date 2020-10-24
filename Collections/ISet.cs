using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Set abstraction.
    /// </summary>
    public interface ISet<Key> : IEnumerable<Key>
    {
        /// <summary>
        /// Adds the key to set if it is not already present.
        /// </summary>
        void Add(Key key);

        /// <summary>
        /// Returns true if the set contains the key; false otherwise.
        /// </summary>
        bool Contains(Key key);

        /// <summary>
        /// Removes the key from the set;
        /// raises <exception cref="System.Collections.Generic.KeyNotFoundException"> if it is not present.
        /// </summary>
        void Remove(Key key);

        bool IsEmpty();

        /// <summary>
        /// Returns the number of keys in the set.
        /// </summary>
        int Size();
    }
}
