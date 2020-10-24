using System;

namespace Collections
{
    /// <summary>
    /// Data structure that uses the weighted quick union algorithm to solve the Dynamic Connectivity problem.
    /// </summary>
    public class QuickUnion<N> : IUnionFind<N>
    {
        private readonly HashMap<N, int> rank = new HashMap<N, int>(); // rank by size; rank[i] is the size of the tree rooted at i
        private readonly HashMap<N, N> id = new HashMap<N, N>(); // id[i] is the parent of i in their connected cpt. tree

        public int Size() => id.Size();

        /// <summary>
        /// Create a <code>QuickUnion</code> object for the given set of (hashable) nodes.
        /// </summary>
        public QuickUnion(params N[] nodes)
        {
            // Initially there are n trees (singleton components) in the forest
            foreach (var n in nodes)
            {
                id[n] = n;
                rank[n] = 1;
            }
        }

        private N Root(N k)
        {
            while (!k.Equals(id[k]))
            {
                id[k] = id[id[k]];  // Partial path compression
                k = id[k];
            }  // A root node is its own parent
            return k;
        }

        public bool Connected(N i, N j)
        {
            if (i == null || j == null)
                throw new ArgumentNullException();

            if (!id.Contains(i) || !id.Contains(j))
                throw new ArgumentException();

            return Root(i).Equals(Root(j));
        }

        public void Union(N i, N j)
        {
            if (i == null || j == null)
                throw new ArgumentNullException();

            if (!id.Contains(i) || !id.Contains(j))
                throw new ArgumentException();

            var rootI = Root(i);
            var rootJ = Root(j);
            if (rootI.Equals(rootJ)) return;  // i and j are already connected

            if (rank[rootI] > rank[rootJ])
            {
                rank[rootI] += rank[rootJ];
                id[rootJ] = rootI;
            }
            else
            {
                rank[rootJ] += rank[rootI];
                id[rootI] = rootJ;
            }  // Prevent tall trees by connecting the smallest cpt. to the largest
        }
    }
}
