using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Graph API to support both directed and undirected graphs.
    /// </summary>
    public interface IGraph<Vertex>
    {
        /// <summary>
        /// Adds the given vertex into the graph if it is not already present.
        /// </summary>
        void Add(Vertex vertex);

        /// <summary>
        /// Adds the given edge into the graph if it is not already present.
        /// </summary>
        void Add(Edge<Vertex> edge);

        /// <summary>
        /// Returns true if the graph contains the given vertex; false otherwise.
        /// </summary>
        bool Contains(Vertex vertex);

        /// <summary>
        /// Returns true if the graph contains the given edge; false otherwise.
        /// </summary>
        bool Contains(Edge<Vertex> edge);

        /// <summary>
        /// Removes the vertex from the graph, and any connecting edges;
        /// raises <exception cref="System.InvalidOperationException"> if it is not present.
        /// </summary>
        void Remove(Vertex vertex);

        /// <summary>
        /// Removes the edge from the graph;
        /// raises <exception cref="System.InvalidOperationException"> if it is not present.
        /// </summary>
        void Remove(Edge<Vertex> edge);

        /// <summary>
        /// Returns true if the graph contains no vertices; false otherwise.
        /// </summary>
        bool IsEmpty();

        /// <summary>
        /// Returns the number of vertices in the graph.
        /// </summary>
        int Size();

        /// <summary>
        /// Returns all the vertices of this graph.
        /// </summary>
        IEnumerable<Vertex> Vertices();

        /// <summary>
        /// Returns all the edges of this graph.
        /// </summary>
        IEnumerable<Edge<Vertex>> Edges();

        /// <summary>
        /// Returns all the outgoing edges from this vertex.
        /// </summary>
        IEnumerable<Edge<Vertex>> Edges(Vertex vertex);
    }
}
