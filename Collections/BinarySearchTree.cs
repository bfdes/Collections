using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    /// <summary>
    /// Binary tree in symmetric order.
    /// </summary>
    public class BinarySearchTree<Key, Value> : ITree<Key, Value> where Key : IComparable<Key>
    {
        // Nodes of a binary tree can either be empty or contain left and right subtrees
        // We say a binary tree is in symmetric order if the key of each node is
        // - larger than all the keys in its left subtree
        // - smaller than all the keys in its right subtree
        // n.b. Duplicate keys are excluded.
        private Node root;

        private class Node
        {
            public Key key;
            public Value value;
            public Node left, right;
            public int size = 1;  // size of the tree rooted at this node
        }

        public Value Get(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            Node current = root;

            while (current != null)
            {
                var diff = key.CompareTo(current.key);
                if (diff == 0)
                {
                    break;
                }
                else if (diff < 0)
                {
                    current = current.left;
                }
                else
                {
                    current = current.right;
                }
            }

            if (current == null)
            {
                throw new KeyNotFoundException();  // Not found
            }
            else
            {
                return current.value;
            }
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

        public bool Contains(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            Node current = root;

            while (current != null)
            {
                var diff = key.CompareTo(current.key);
                if (diff == 0)
                {
                    return true;
                }
                else if (diff < 0)
                {
                    current = current.left;
                }
                else
                {
                    current = current.right;
                }
            }
            return false;
        }

        public void Delete(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            root = Delete(root);

            Node Delete(Node n)
            {
                if (n == null)
                {
                    throw new KeyNotFoundException();  // Not found
                }

                var diff = key.CompareTo(n.key);
                if (diff < 0)
                {
                    n.left = Delete(n.left);
                }
                else if (diff > 0)
                {
                    n.right = Delete(n.right);
                }
                else
                {
                    // 1) A leaf: return null 2) Has one child: move it upwards
                    if (n.left == null) return n.right;
                    if (n.right == null) return n.left;
                    // 3) Has two children
                    var min = Min(n.right);
                    (n.key, n.value) = (min.key, min.value);
                    n.right = DeleteMin(n.right);
                }
                n.size = Size(n.left) + Size(n.right) + 1;
                return n;
            }

            // Deletes the smallest node in the subtree rooted at n and returns the modified tree
            Node DeleteMin(Node n)
            {
                // n.b. No parent pointer, so has to be written recursively.
                if (n.left == null)
                {
                    return n.right;
                }
                n.left = DeleteMin(n.left);
                n.size = Size(n.left) + Size(n.right) + 1;
                return n;
            }
        }

        public void Put(Key key, Value value)
        {
            if (key == null)
                throw new ArgumentNullException();

            Node Put(Node n)
            {
                if (n == null)
                {
                    return new Node { key = key, value = value };  // Not found
                }

                var diff = key.CompareTo(n.key);
                if (diff == 0)
                {
                    n.value = value; // Found, overwrite value.
                }
                else if (diff < 0)
                {
                    n.left = Put(n.left);
                }
                else
                {
                    n.right = Put(n.right);
                }
                n.size = Size(n.left) + Size(n.right) + 1;

                return n;
            }
            root = Put(root);
        }

        private Node Min(Node n)
        {
            Node current = n;
            while (current.left != null)
            {
                current = current.left;
            }
            return current;
        }

        public Key Min()
        {
            if (root == null)
            {
                throw new InvalidOperationException();
            }
            return Min(root).key;
        }

        public Key Max()
        {
            if (root == null)
            {
                throw new InvalidOperationException();
            }

            Node current = root;
            Key max;

            do
            {
                max = current.key;
                current = current.right;
            } while (current != null);

            return max;
        }

        public Key Floor(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            // Adapted from Algorithms I. "Tricky code."
            Key Floor(Node n)
            {
                if (n == null) throw new InvalidOperationException();

                var diff = key.CompareTo(n.key);
                if (diff == 0)
                {
                    return key;
                }

                try
                {
                    return diff < 0 ? Floor(n.left) : Floor(n.right);
                }
                catch (InvalidOperationException)
                {
                    return n.key;
                }
            }
            return Floor(root);
        }

        public Key Ceiling(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            Key Ceiling(Node n)
            {
                if (n == null) throw new InvalidOperationException();

                var diff = key.CompareTo(n.key);
                if (diff == 0)
                {
                    return key;
                }

                try
                {
                    return diff < 0 ? Ceiling(n.left) : Ceiling(n.right);
                }
                catch (InvalidOperationException)
                {
                    return n.key;
                }
            }
            return Ceiling(root);
        }

        public int Rank(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            int Rank(Node node)
            {
                if (node == null) return 0;
                var diff = key.CompareTo(node.key);
                if (diff == 0)
                {
                    return Size(node.left);
                }
                else if (diff < 0)
                {
                    return Rank(node.left);
                }
                else
                {
                    return 1 + Size(node.left) + Rank(node.right);
                }
            }
            return Rank(root);
        }

        private static int Size(Node node) => node == null ? 0 : node.size;

        public int Size() => Size(root);

        public bool IsEmpty() => root == null;

        public IEnumerable<(Key key, Value value)> Pairs()
        {
            var stack = new LinkedList<Node>();
            Node current = root;

            while (current != null || !stack.IsEmpty())
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.left;
                }
                current = stack.Pop();
                yield return (current.key, current.value);
                current = current.right;
            }
        }

        public IEnumerable<Key> Keys() => Pairs().Select(t => t.key);

        public IEnumerable<Value> Values() => Pairs().Select(t => t.value);

        public bool IsSymmetric()
        {
            bool IsSymmetric(Node node)
            {
                var leftSymmetric =
                    node.left == null ? true : node.key.CompareTo(node.left.key) > 0 && IsSymmetric(node.left);
                var rightSymmetric =
                    node.right == null ? true : node.key.CompareTo(node.right.key) < 0 && IsSymmetric(node.right);
                return leftSymmetric && rightSymmetric;
            }
            return root == null ? true : IsSymmetric(root);
        }
    }
}
