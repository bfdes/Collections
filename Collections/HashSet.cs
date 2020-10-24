using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Set implementation using a hash table. 
    /// </summary>
    public class HashSet<Key> : ISet<Key>
    {
        private int n = 0;  // The number of keys in the table (cached for performance)
        private int m;  // The number of chains, i.e. table.Length (written this way for convenience)
        private Node[] table;

        private class Node
        {
            public readonly Key key;  // key must only be assigned once
            public Node next;

            public Node(Key key, Node next)
            {
                this.key = key;
                this.next = next;
            }
        }

        public HashSet(int capacity = 4)
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
            var temp = new HashSet<Key>(size);
            foreach (var key in this)
            {
                temp.Add(key);
            }
            table = temp.table;
            m = temp.m;  // n.b. n should be unchanged
        }

        private int Hash(Key key) => (key.GetHashCode() & 0x7fffffff) % m;

        public void Add(Key key)
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
                    return;
                }
                current = current.next;
            }
            // Not found
            n++;
            table[i] = new Node(key, table[i]);
        }

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

        public IEnumerator<Key> GetEnumerator()
        {
            Node node;

            foreach (var chain in table)
            {
                node = chain;
                while (node != null)
                {
                    yield return node.key;
                    node = node.next;
                }
            }
        }

        public bool IsEmpty() => n == 0;

        public void Remove(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            Node Remove(Node node)
            {
                if (node == null)
                    throw new KeyNotFoundException();
                if (key.Equals(node.key))
                {
                    n--;
                    return node.next;
                }
                node.next = Remove(node.next);
                return node;
            }

            var i = Hash(key);
            table[i] = Remove(table[i]);

            // Halve the table size if the average length of a chain is <= 2
            if (m > 4 && n <= 2 * m) Resize(m / 2);
        }

        public int Size() => n;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
