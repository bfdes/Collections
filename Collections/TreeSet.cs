using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Collections
{
    public class TreeSet<Key> : ISet<Key> where Key : IComparable<Key>
    {
        private Node root;
        private enum Color { Red, Black };

        private class Node
        {
            public Key key;
            public Color color;  // Color of the incoming link
            public Node left;
            public Node right;
            public int size = 1;

            // Invariants:
            // - No path contains more than two consecutive red links
            // - The no. of black links on every path is the same

            // Used transform a 3 node whose red link leans to the right to one in which the red link leans to the left
            public Node RotateLeft()
            {
                Debug.Assert(IsRed(this.right));
                Node m = this.right;
                (this.right, m.left) = (m.left, this);
                (m.color, this.color) = (this.color, Color.Red);
                (m.size, this.size) = (this.size, Size(this.left) + Size(this.right) + 1);
                return m;
            }

            public Node RotateRight()
            {
                Debug.Assert(IsRed(this.left));
                Node n = this.left;
                (n.right, this.left) = (this, n.right);
                (n.color, this.color) = (this.color, Color.Red);
                (n.size, this.size) = (this.size, Size(this.left) + Size(this.right) + 1);
                return n;
            }

            // Used to split a '4' node into two 2 nodes; may introduce two consecutive red links higher in the tree
            public Node FlipColors()
            {
                var WasRed = color == Color.Red;
                Debug.Assert(left.color == (WasRed ? Color.Black : Color.Red));
                Debug.Assert(left.color == (WasRed ? Color.Black : Color.Red));

                color = WasRed ? Color.Black : Color.Red;
                left.color = WasRed ? Color.Red : Color.Black;
                right.color = WasRed ? Color.Red : Color.Black;
                return this;
            }

            public Node MoveLeft()
            {
                FlipColors();
                if (IsRed(right.left))
                {
                    right = right.RotateRight();
                    var n = RotateLeft();
                    return n.FlipColors();
                }
                return this;
            }

            public Node MoveRight()
            {
                FlipColors();
                if (IsRed(left.left))
                {
                    var n = RotateRight();
                    return n.FlipColors();
                }
                return this;
            }
        }

        private Node Balance(Node n)
        {
            if (IsRed(n.right) && !IsRed(n.left)) n = n.RotateLeft();
            if (IsRed(n.left) && IsRed(n.left.left)) n = n.RotateRight();
            if (IsRed(n.left) && IsRed(n.right)) n.FlipColors();
            n.size = Size(n.left) + Size(n.right) + 1;
            return n;
        }

        private static bool IsRed(Node n) =>
            n == null ? false : n.color == Color.Red;  // Cannot be written as an extension method

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

        public void Remove(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsRed(root.left) && !IsRed(root.right))
            {
                root.color = Color.Red;
            }

            root = Remove(root);

            if (!IsEmpty())
            {
                root.color = Color.Black;
            }

            Node Remove(Node n)
            {
                if (n == null)
                {
                    throw new KeyNotFoundException();  // Not found
                }

                var diff = key.CompareTo(n.key);
                if (diff < 0)
                {
                    if (!IsRed(n.left) && !IsRed(n.left.left))
                    {
                        n = n.MoveLeft();
                    }
                    n.left = Remove(n.left);
                }
                else
                {
                    if (IsRed(n.left))
                    {
                        n = n.RotateRight();
                    }

                    if (diff == 0 && (n.right == null))
                    {
                        return null;
                    }

                    if (!IsRed(n.right) && !IsRed(n.right.left))
                    {
                        n = n.MoveRight();
                    }

                    if (diff == 0)
                    {
                        var min = Min(n.right);
                        n.key = min.key;
                        n.right = DeleteMin(n.right);
                    }
                    else
                    {
                        n.right = Remove(n.right);
                    }
                }

                return Balance(n);
            }

            Node DeleteMin(Node n)
            {
                if (n.left == null)
                {
                    return null;
                }

                if (!IsRed(n.left) && !IsRed(n.left.left))
                {
                    n = n.MoveLeft();
                }

                n.left = DeleteMin(n.left);

                return Balance(n);
            }
        }

        public void Add(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            Node Add(Node n)
            {
                if (n == null)
                {
                    return new Node { key = key, color = Color.Red };
                }
                var diff = key.CompareTo(n.key);
                if (diff < 0)
                {
                    n.left = Add(n.left);
                }
                else if (diff > 0)
                {
                    n.right = Add(n.right);
                }

                return Balance(n);
            }

            root = Add(root);
            root.color = Color.Black;
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

        public IEnumerator<Key> GetEnumerator()
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
                yield return current.key;
                current = current.right;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
