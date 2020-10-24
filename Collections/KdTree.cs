using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Collections.Sorting;

namespace Collections
{
    public class KdTree : IEnumerable
    {
        // A KdTree or k-dimensional tree is a natural extension of a binary search tree.
        //
        // Each node of the tree defines a 'cutting dimension' with which it partitions its children.
        // Additionally, in this implementation, each node of the tree also defines a point.
        // Thus every point lies on a splitting axis (another option is to store the points in the leaves only).
        // As we traverse the levels of the tree we cycle through the dimensions of space partitioned by it.

        private readonly int dimension;
        private Node root;

        private class Node
        {
            public Point point;
            public Node left;
            public Node right;
        }

        /// <summary>
        /// Uses the Round-Robin strategy to create a KdTree of the given dimension for the set of points.
        /// </summary>
        public KdTree(params Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            if (!points.Any())
                throw new ArgumentOutOfRangeException();

            dimension = points.First().Dimension();

            if (points.Any(p => p.Dimension() != dimension))
                throw new ArgumentException();

            Shuffle(points);

            // Partitions the points in the range [low, high] into a subtree defined by the cutting dimension l
            Node KdTree(int low, int high, int l)
            {
                if (high < low)
                {
                    return null;
                }

                var byLevel = new Point.ByDimension(l);
                QuickSort(points, low, high, byLevel);
                var mid = low + (high - low) / 2;
                var newL = (l + 1) % dimension;

                return new Node
                {
                    point = points[mid],
                    left = KdTree(low, mid - 1, newL),
                    right = KdTree(mid + 1, high, newL)
                };
            }

            root = KdTree(0, points.Length - 1, 0);
        }

        /// <summary>
        /// Creates an empty KdTree of the given dimension.
        /// </summary>
        public KdTree(int dimension = 3)
        {
            this.dimension = dimension;
        }

        public bool IsEmpty() => root == null;

        public bool Contains(Point p)
        {
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            if (p.Dimension() != dimension)
            {
                throw new ArgumentException();
            }

            bool Contains(Node n, int l)
            {
                if (n == null) return false;

                if (n.point == p) return true;

                var diff = p[l] - n.point[l];

                if (diff < 0)
                {
                    return Contains(n.left, (l + 1) % dimension);
                }
                else
                {
                    return Contains(n.right, (l + 1) % dimension);
                }
            }
            return Contains(root, 0);
        }

        /// <summary>
        /// Put a point p into the tree if it does not already exist.
        /// </summary>
        public void Put(Point p)
        {
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            if (p.Dimension() != dimension)
            {
                throw new ArgumentException();
            }

            // Puts the point p into the tree rooted at n
            // We pass down the 'level' (plane of insertion / cutting dimension) at every step.
            Node Put(Node n, int l)
            {
                if (n == null)
                {
                    return new Node { point = p };  // Not found 
                }

                if (n.point == p)
                {
                    return n; // Found
                }

                var diff = p[l] - n.point[l];

                // Put the point on the correct side of the plane
                if (diff < 0)
                {
                    return Put(n.left, (l + 1) % dimension);
                }
                else
                {
                    return Put(n.right, (l + 1) % dimension);
                }
            }

            root = Put(root, 0);
        }

        /// <summary>
        /// Remove a point <code>p</code> from the tree.
        /// raises <exception cref="System.InvalidOperationException"> if the point does not exist.
        /// </summary>
        public void Delete(Point p)
        {
            // Implementation ported from CMU Bioinformatics lecture slides (offered by Carl Kingsford)
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            if (p.Dimension() != dimension)
            {
                throw new ArgumentException();
            }

            // Returns the smallest point wrt to the given dimension d 
            // in the tree rooted at n (here l is the cutting dimension of n)
            Point Min(Node n, int d, int l)
            {
                if (n == null) return null;

                var newL = (l + 1) % dimension;

                if (l == d)
                {
                    // If we are on the correct plane then we only need to inspect the left branch
                    if (n.left == null)
                    {
                        return n.point;
                    }
                    else
                    {
                        return Min(n.left, d, newL);
                    }
                }
                else
                {
                    // Otherwise we need to take a look at both branches
                    var leftPt = Min(n.left, d, newL);
                    var rightPt = Min(n.right, d, newL);
                    var byDim = new Point.ByDimension(d);

                    if (leftPt == null && rightPt == null)
                    {
                        return n.point;
                    }
                    else if (leftPt == null)
                    {
                        return byDim.Min(n.point, rightPt);
                    }
                    else if (rightPt == null)
                    {
                        return byDim.Min(n.point, leftPt);
                    }
                    else
                    {
                        return byDim.Min(n.point, leftPt, rightPt);
                    }
                }
            }

            // Delete q from the tree rooted at n (here l is the cutting dimension of n), and return the new subtree
            Node Delete(Point q, Node n, int l)
            {
                if (n == null)
                {
                    throw new InvalidOperationException();  // Not found (it is OK to throw here)
                }
                var newL = (l + 1) % dimension;

                if (n.point == q)
                {
                    // Found the point
                    if (n.right != null)
                    {
                        // Replace the point with the smallest one in the right branch of n
                        n.point = Min(n.right, l, newL);  // n.b. Cannot be null, by design
                        n.right = Delete(n.point, n.right, newL);
                    }
                    else if (n.left != null)
                    {
                        // Only have a left branch, so move its contents to the right
                        n.point = Min(n.left, l, newL);
                        n.right = Delete(n.point, n.left, newL);
                        n.left = null;
                    }
                    else
                    {
                        n = null;
                    }
                }
                else if (q[l] < n.point[l])
                {
                    n.left = Delete(q, n.left, newL);
                }
                else
                {
                    n.right = Delete(q, n.right, newL);
                }
                return n;
            }
            root = Delete(p, root, 0);
        }

        /// <summary> 
        /// Returns all the points of the tree within the given query rectangle.
        /// </summary>
        public IEnumerable<Point> RangeSearch(Rectangle r)
        {
            if (r == null)
            {
                throw new ArgumentNullException();
            }

            if (r.Dimension() != dimension)
            {
                throw new ArgumentException();
            }

            IEnumerable<Point> RangeSearch(Node n, Rectangle bb, int l)
            {
                // Only search the tree rooted at this node if
                // i) it isn't empty
                // ii) its bounding box intersects with the query rectangle
                if (n == null || !r.Intersects(bb))
                {
                    yield break;
                }

                if (r.Contains(n.point))
                {
                    yield return n.point;
                }

                // Calculate the bounding boxes of the children
                var lBb = bb.SetRight(l, n.point[l]);
                var rBb = bb.SetLeft(l, n.point[l]);
                // And their cutting-dimension
                var newL = (l + 1) % dimension;

                foreach (var point in RangeSearch(n.left, lBb, newL))
                {
                    yield return point;
                }

                foreach (var point in RangeSearch(n.right, rBb, newL))
                {
                    yield return point;
                }
            }

            return RangeSearch(root, Rectangle.InfiniteBox(dimension), 0);
        }

        /// <summary>
        /// Returns the closest point to p in this tree.
        /// raises <exception cref="System.InvalidOperationException"> if it is empty.
        /// </summary>
        public Point NearestNeighbour(Point p)
        {
            // Implementation taken from CMU Bioinformatics lecture slides (offered by Carl Kingsford)
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            if (p.Dimension() != dimension)
            {
                throw new ArgumentException();
            }

            if (IsEmpty())
            {
                throw new InvalidOperationException();
            }

            // Closest point seen so far
            Point closest = null;
            var min = double.MaxValue;

            void Go(Node n, Rectangle bb, int l)
            {
                // Investigate a branch only if its bounding box is likely to contain a closer point to that already found.
                if (n == null || bb.DistanceSquaredTo(p) >= min)
                {
                    return;
                }

                // d^2 is monotonic on d and cheaper to compute
                var dist = p.DistanceSquaredTo(n.point);
                if (dist < min)
                {
                    min = dist;
                    closest = n.point;
                }

                // Investigate the branches in the order that is likely to enable rapid pruning of the search path
                var lBb = bb.SetRight(l, p[l]);
                var rBb = bb.SetLeft(l, p[l]);
                var newL = (l + 1) % dimension;

                if (p[l] < n.point[l])
                {
                    Go(n.left, lBb, newL);
                    Go(n.right, rBb, newL);
                }
                else
                {
                    Go(n.right, rBb, newL);
                    Go(n.left, lBb, newL);
                }
            }

            Go(root, Rectangle.InfiniteBox(dimension), 0);
            return closest;
        }

        public bool IsSymmetric()
        {
            bool IsSymmetric(Node n, int l)
            {
                var newL = (l + 1) % dimension;

                var leftSymmetric = n.left == null
                    ? true
                    : n.point[l] - n.left.point[l] > 0 && IsSymmetric(n.left, newL);

                var rightSymmetric = n.right == null
                    ? true
                    : n.point[l] - n.right.point[l] <= 0 && IsSymmetric(n.right, newL);

                return leftSymmetric && rightSymmetric;
            }
            return root == null ? true : IsSymmetric(root, 0);
        }

        public IEnumerator GetEnumerator()
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
                yield return current.point;
                current = current.right;
            }
        }
    }
}
