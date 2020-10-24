namespace Collections
{
    /// <summary>
    /// Data structure to encapsulate an algorithm that solves the Dynamic Connectivity problem.
    /// </summary>
    public interface IUnionFind<N>
    {
        /// <summary>
        /// Returns the number of nodes in the object.
        /// </summary>
        int Size();

        /// <summary>
        /// Returns true if i and j are part of the same connected component, false otherwise.
        /// </summary>
        bool Connected(N i, N j);  // n.b. an equivalence relation

        /// <summary>
        /// Adds the nodes i and j to the same connected component.
        /// </summary>
        void Union(N i, N j);
    }
}
