using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Collections
{
    public class TernarySearchTrie<Value> : ITrie<Value> where Value : class
    {
        private Node root;
        private int size;

        private class Node
        {
            public char key;
            public Value value;
            public Node left;
            public Node middle;
            public Node right;
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
                var c = key[depth];

                if (c < node.key)
                    node.left = Delete(node.left, depth);
                else if (c > node.key)
                    node.right = Delete(node.right, depth);
                else if (depth < key.Length - 1)
                    node.middle = Delete(node.middle, depth + 1);
                else if (node.value == null)
                    throw new KeyNotFoundException();
                else
                    node.value = null;
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
                return null;  // Not found
            var c = key[depth];
            if (c < node.key)
                return Get(node.left, key, depth);
            else if (c > node.key)
                return Get(node.right, key, depth);
            else if (depth < key.Length - 1)
                return Get(node.middle, key, depth + 1);
            return node;  // node.value could be null if 'key' is merely a prefix of another
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
                var c = key[depth];
                if (node == null)
                    node = new Node { key = c };
                if (c < node.key)
                    node.left = Put(node.left, depth);
                else if (c > node.key)
                    node.right = Put(node.right, depth);
                else if (depth < key.Length - 1)
                    node.middle = Put(node.middle, depth + 1);
                else
                    node.value = value;
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
            Collect(node.left, sb, queue);
            sb.Append(node.key);
            if (node.value != null)
                queue.Push((sb.ToString(), node.value));
            Collect(node.middle, sb, queue);
            sb.Length--;
            Collect(node.right, sb, queue);
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

                if (c < node.key)
                    return LongestPrefix(node.left, d, l);
                else if (c > node.key)
                    return LongestPrefix(node.right, d, l);
                return LongestPrefix(node.middle, d + 1, l);
            }
            var length = LongestPrefix(root, 0, 0);
            return query.Substring(0, length);
        }
    }
}
