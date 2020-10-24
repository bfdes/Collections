using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    /// <summary>
    /// Associative array abstraction that supports constant time access to key-value pairs. 
    /// Uses the separate chaining strategy to account for key hash collisions.
    /// </summary>
    public class HashMap<Key, Value> : IMap<Key, Value>
    {
        // Implementation ported from Algorithms I by Sedgewick and Wayne

        private int n = 0;  // The number of key-value pairs in the table (cached for performance)
        private int m;  // The number of chains, i.e. table.Length (written this way for convenience)
        private Node[] table;

        private class Node
        {
            public Key key;
            public Value value;
            public Node next;
        }

        public Value this[Key key]
        {
            get
            {
                return Get(key);
            }

            set
            {
                Put(key, value);
            }
        }

        public HashMap(int capacity = 4)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            table = new Node[capacity];
            m = capacity;
        }

        private void Resize(int size)
        {
            // We cannot simply clone the table as <code>Hash</code> function changes 
            var temp = new HashMap<Key, Value>(size);
            foreach (var (key, value) in Pairs())
            {
                temp.Put(key, value);
            }
            table = temp.table;
            m = temp.m;  // n.b. n should be unchanged
        }

        private int Hash(Key key) => (key.GetHashCode() & 0x7fffffff) % m;

        public bool Contains(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            var i = Hash(key);
            var current = table[i];

            while (current != null)
            {
                if (key.Equals(current.key))
                {
                    return true;
                }
                current = current.next;
            }
            return false;
        }

        public void Delete(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            Node Delete(Node node)
            {
                if (node == null)
                    throw new KeyNotFoundException();
                if (key.Equals(node.key))
                {
                    n--;
                    return node.next;
                }
                node.next = Delete(node.next);
                return node;
            }

            var i = Hash(key);
            table[i] = Delete(table[i]);

            // Halve the table size if the average length of a chain is <= 2
            if (m > 4 && n <= 2 * m) Resize(m / 2);
        }

        public void Put(Key key, Value value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            // Double the table size if the average length of a chain is >= 10
            if (n >= 10 * m) Resize(2 * m);

            var i = Hash(key);
            var current = table[i];

            while (current != null)
            {
                if (key.Equals(current.key))
                {
                    current.value = value;
                    return;
                }
                current = current.next;
            }
            // Not found
            n++;
            table[i] = new Node { key = key, value = value, next = table[i] };
        }

        public Value Get(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            var i = Hash(key);
            var current = table[i];

            while (current != null)
            {
                if (key.Equals(current.key))
                {
                    break;
                }
                current = current.next;
            }

            if (current == null)
            {
                throw new KeyNotFoundException();
            }
            return current.value;
        }

        public Value GetOrElse(Key key, Value defaultValue)
        {
            try
            {
                return Get(key);
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        public bool IsEmpty() => n == 0;

        public IEnumerable<Key> Keys() => Pairs().Select(p => p.key);

        public IEnumerable<(Key key, Value value)> Pairs()
        {
            Node node;

            foreach (var chain in table)
            {
                node = chain;
                while (node != null)
                {
                    yield return (node.key, node.value);
                    node = node.next;
                }
            }
        }

        public int Size() => n;

        public IEnumerable<Value> Values() => Pairs().Select(p => p.value);
    }
}
