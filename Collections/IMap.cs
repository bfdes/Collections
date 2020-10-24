using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Symbol table abstraction.
    /// </summary>
    public interface IMap<Key, Value>
    {
        /// <summary>
        /// Places the given key-value pair into the table, overwriting an existing pair if necessary.
        /// </summary>
        void Put(Key key, Value value);

        /// <summary>
        /// Returns the value paired with the given key;
        /// raises <exception cref="System.Collections.Generic.KeyNotFoundException"> if it is not present.
        /// </summary>
        Value Get(Key key);

        /// <summary>
        /// Returns the value paired with the given key, or the default if it is not present.
        /// </summary>
        Value GetOrElse(Key key, Value defaultValue);

        /// <summary>
        /// Deletes the given key and its value from the table;
        /// raises <exception cref="System.Collections.Generic.KeyNotFoundException"> if it is not present.
        /// </summary>
        void Delete(Key key);

        /// <summary>
        /// Returns true if the table contains the given key; false otherwise.
        /// </summary>
        bool Contains(Key key);

        /// <summary>
        /// Returns true if the table contains no key-value pairs; false otherwise.
        /// </summary>
        bool IsEmpty();

        /// <summary>
        /// Returns the number of key-value pairs in the table.
        /// </summary>
        int Size();

        /// <summary>
        /// Returns the key-value pairs of this map;
        /// </summary>
        IEnumerable<(Key key, Value value)> Pairs();

        /// <summary>
        /// Returns the keys of this map.
        /// </summary>
        IEnumerable<Key> Keys();

        /// <summary>
        /// Returns the values of this map.
        /// </summary>
        IEnumerable<Value> Values();
    }
}
