using System;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Tree data structure containing key-value pairs in its nodes.
    /// Keys support a total ordering.
    /// </summary>
    public interface ITree<Key, Value> where Key : IComparable<Key>
    {
        /// <summary>
        /// Places the given key-value pair into the tree, overwriting an existing pair if necessary.
        /// </summary>
        void Put(Key key, Value value);

        /// <summary>
        /// Returns the value paired with the given key;
        /// raises <exception cref="System.InvalidOperationException"> if it is not present.
        /// </summary>
        Value Get(Key key);

        /// <summary>
        /// Returns the value paired with the given key, or the default if it is not present.
        /// </summary>
        Value GetOrElse(Key key, Value defaultValue);

        /// <summary>
        /// Deletes the given key and its value from the tree;
        /// raises <exception cref="System.InvalidOperationException"> if it is not present.
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
        /// Returns the number of key-value pairs in the tree.
        /// </summary>
        int Size();

        /// <summary>
        /// Returns the smallest key in the tree;
        /// raises <exception cref="System.InvalidOperationException"> if the tree is empty.
        /// </summary>
        Key Min();

        /// <summary>
        /// Returns the largest key in the tree;
        /// raises <exception cref="System.InvalidOperationException"> if the tree is empty.
        /// </summary>
        Key Max();

        /// <summary>
        /// Largest key in the tree smaller than or equal to the given key if there is one,
        /// raises <exception cref="System.InvalidOperationException"> otherwise.
        /// </summary>
        Key Floor(Key key);

        /// <summary>
        /// Smallest key in the tree larger than or equal to the given key if there is one,
        /// raises <exception cref="System.InvalidOperationException"> otherwise.
        /// </summary>
        Key Ceiling(Key key);

        /// <summary>
        /// The number of keys in the tree strictly less than this key.
        /// </summary>
        int Rank(Key key);

        /// <summary>
        /// Returns the key-value pairs of this tree inorder;
        /// </summary>
        IEnumerable<(Key key, Value value)> Pairs();

        /// <summary>
        /// Returns the keys of this tree inorder.
        /// </summary>
        IEnumerable<Key> Keys();

        /// <summary>
        /// Returns the values of this tree inorder.
        /// </summary>
        IEnumerable<Value> Values();

        bool IsSymmetric();
    }
}
