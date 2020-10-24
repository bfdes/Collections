using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Collections
{
    public class RadixTrie<Value> : ITrie<Value> where Value : class
    {
        public static readonly int r = 256;
        private Node root = new Node();
        private int size = 0;

        private class Node
        {
            public Value value;
            public Node[] children = new Node[r];

            public Node(Value value)
            {
                this.value = value;
            }

            public Node() { }
        }

        public Value this[string key]
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

        public bool Contains(string key)
        {
            if (key == null)
                throw new ArgumentNullException();
            var node = Get(root, key, 0);
            return node != null && node.value != null;
        }

        public void Delete(string key)
        {
            if (key == null)
                throw new ArgumentNullException();

            Node Delete(Node node, int depth)
            {
                if (node == null)
                    throw new KeyNotFoundException();
                if (depth == key.Length)
                {
                    node.value = null;
                }
                else
                {
                    var c = key[depth];
                    node.children[c] = Delete(node.children[c], depth + 1);
                }

                // Optimisation: Provided we are not at the root node we may be able to get rid of this node if it carries no values
                if (depth == 0)
                {
                    return node;
                }
                else if (node.value == null && node.children.All(child => child == null))
                {
                    return null;
                }

                return node;
            }

            root = Delete(root, 0);
            size--;
        }

        public Value Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException();
            var node = Get(root, key, 0);
            if (node == null || node.value == null)
                throw new KeyNotFoundException();
            return node.value;
        }

        private Node Get(Node node, string key, int depth)
        {
            if (node == null)
                return null;
            if (depth == key.Length)
                return node;  // node.value could be null if 'key' is merely a prefix of another
            return Get(node.children[key[depth]], key, depth + 1);
        }

        public Value GetOrElse(string key, Value defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException();

            var node = Get(root, key, 0);
            if (node == null || node.value == null)
                return defaultValue;
            return node.value;
        }

        public bool IsEmpty() => size == 0;

        public IEnumerable<string> Keys() => Pairs().Select(p => p.key);

        public IEnumerable<string> Keys(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException();

            var queue = new Queue<(string key, Value value)>();
            var node = Get(root, prefix, 0);  // Root of subtree for all trees begining with the given prefix
            var sb = new StringBuilder(prefix);
            Collect(node, sb, queue);
            return queue.Select(p => p.key);
        }

        public IEnumerable<(string key, Value value)> Pairs()
        {
            var queue = new Queue<(string key, Value value)>();
            var sb = new StringBuilder();
            Collect(root, sb, queue);
            return queue;
        }

        public void Put(string key, Value value)
        {
            if (key == null || value == null)
                throw new ArgumentNullException();

            Node Put(Node node, int depth)
            {
                if (node == null)
                    node = new Node();
                if (depth == key.Length)
                {
                    node.value = value;
                    return node;
                }
                var c = key[depth];
                node.children[c] = Put(node.children[c], depth + 1);
                return node;

            }

            root = Put(root, 0);
            size++;
        }

        public int Size() => size;

        public IEnumerable<Value> Values() => Pairs().Select(p => p.value);

        private static void Collect(Node node, StringBuilder sb, Queue<(string key, Value value)> queue)
        {
            if (node == null)
                return;
            if (node.value != null)
                queue.Push((sb.ToString(), node.value));
            for (int i = 0; i < r; i++)
            {
                sb.Append((char)i);
                Collect(node.children[i], sb, queue);
                sb.Length--;
            }
        }

        public string LongestPrefix(string query)
        {
            if (query == null)
                throw new ArgumentNullException();

            int LongestPrefix(Node node, int d, int l)
            {
                if (node == null)
                    return l;
                if (node.value != null)
                    l = d;
                if (d == query.Length)
                    return l;
                int c = query[d];
                return LongestPrefix(node.children[c], d + 1, l);
            }
            var length = LongestPrefix(root, 0, 0);
            return query.Substring(0, length);
        }
    }
}
