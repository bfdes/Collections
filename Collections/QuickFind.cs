using System;

namespace Collections
{
    /// <summary>
    /// Data sturcture that uses the quick find algorithm to solve the Dynamic Connectivity problem.
    /// </summary>
    /// <typeparam name="N">Hashable node</typeparam>
    public class QuickFind<N> : IUnionFind<N>
    {
        private readonly HashMap<N, N> id = new HashMap<N, N>();  // id[i] is the node rooting the cpt. containing i

        public int Size() => id.Size();

        /// <summary>
        /// Create a <code>QuickFind</code> object for the given set of (hashable) nodes.
        /// </summary>
        public QuickFind(params N[] nodes)
        {
            foreach (var n in nodes)
            {
                id[n] = n;  // Each node is initially in its own component.
            }
        }

        public bool Connected(N i, N j)
        {
            if (i == null || j == null)
                throw new ArgumentNullException();

            if (!id.Contains(i) || !id.Contains(j))
                throw new ArgumentException();

            return id[i].Equals(id[j]);
        }

        public void Union(N i, N j)
        {
            if (i == null || j == null)
                throw new ArgumentNullException();

            if (!id.Contains(i) || !id.Contains(j))
                throw new ArgumentException();

            var rootI = id[i];
            var rootJ = id[j];

            if (rootI.Equals(rootJ))
                return;  // i and j are already connected

            foreach (var (k, v) in id.Pairs())
            {
                if (v.Equals(rootI))
                    id[k] = rootJ;
            }
        }
    }
}
