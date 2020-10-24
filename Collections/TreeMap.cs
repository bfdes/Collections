using System;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Map implementation using a <code>RedBlackTree</code>.
    /// </summary>
    class TreeMap<Key, Value> : IMap<Key, Value> where Key : class, IComparable<Key> where Value : class
    {
        private readonly RedBlackTree<Key, Value> tree = new RedBlackTree<Key, Value>();
        public bool Contains(Key key) => tree.Contains(key);

        public void Delete(Key key)
        {
            tree.Delete(key);
        }

        public Value Get(Key key) => tree.Get(key);

        public Value GetOrElse(Key key, Value defaultValue) => tree.GetOrElse(key, defaultValue);

        public bool IsEmpty() => tree.IsEmpty();

        public IEnumerable<Key> Keys() => tree.Keys();

        public IEnumerable<(Key key, Value value)> Pairs() => tree.Pairs();

        public void Put(Key key, Value value)
        {
            tree.Put(key, value);
        }

        public int Size() => tree.Size();

        public IEnumerable<Value> Values() => tree.Values();
    }
}
