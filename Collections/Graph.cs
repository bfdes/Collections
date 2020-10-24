using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    using static Option;

    public static class Graph
    {
        /// <summary>
        /// Returns all the connected components that comprise this <c>graph</c> in O(V+E) time.
        /// </summary>
        /// <returns>A set containing each connected component, represented as a subgraph</returns>
        public static ISet<Graph<V>> Components<V>(Graph<V> graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            var components = new HashSet<Graph<V>>();
            var visited = new HashSet<V>();  // All the vertices that are already in a component

            // Creates and returns a subgraph containing vertices reachable by s only
            Graph<V> Component(V s)
            {
                var g = new Graph<V>();
                var stack = new LinkedList<V>();
                stack.Push(s);

                while (!stack.IsEmpty())
                {
                    var v = stack.Pop();
                    visited.Add(v);
                    foreach (var e in graph.Edges(v))
                    {
                        var w = e.to;
                        if (visited.Contains(w))
                            continue;  // w is already part of THIS component
                        graph.Add(e);
                        stack.Push(w);
                    }
                }

                return g;
            }

            foreach (var v in graph.Vertices())
            {
                if (visited.Contains(v))
                    continue;  // v is part of a component that has already been created 
                components.Add(Component(v));
            }

            return components;
        }

        /// <summary>
        /// Produces a minimum spanning tree of the given graph using Kruskal's algorithm in O(E*ln V) time.
        /// Raises <exception cref="System.InvalidOperationException"> if the graph is not connected.
        /// </summary>
        /// <returns>Graph representation of the MST</returns>
        public static Graph<V> Kruskal<V>(Graph<V> graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            if (!graph.IsConnected())
                throw new InvalidOperationException();  // We would need to produce a minimum spanning forest instead

            // Kruskal's algorithm is a special case of the greedy MST algorithm:
            // - Maintain a forest of trees (initially individual vertices of the graph)
            // - Repreatedly add edges in descending order of weight to combine forests into a tree
            //   (Exclude edges that connect vertices that are already part of the same tree.)

            var tree = new Graph<V>();
            var n = 0;  // no. of vertices in the MST
            var queue = new BinaryHeap<Edge<V>>();

            foreach (var e in graph.Edges())
                queue.Push(e);

            var vertices = graph.Vertices();
            var N = vertices.Count();  // no. of vertices in the original graph
            var uf = new QuickUnion<V>(vertices.ToArray());

            // The MST of a connected graph must contain exactly N-1 edges
            while (n < N - 1)
            {
                var e = queue.Pop();
                if (!uf.Connected(e.from, e.to))
                {
                    uf.Union(e.from, e.to);
                    tree.Add(e);
                    n++;
                }
            }
            return tree;
        }

        /// <summary>
        /// Produces a minimum spanning tree of the given graph using Prim's algorithm in O(E*ln E) time.
        /// Raises <exception cref="System.InvalidOperationException"> if the graph is not connected.
        /// </summary>
        /// <returns>Graph representation of the MST</returns>
        public static Graph<V> Prim<V>(Graph<V> graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            if (!graph.IsConnected())
                throw new InvalidOperationException();

            // Lazy variant of Prim's algorithm:
            // Grow a tree by adding the smallest edge pointing out of it
            var tree = new Graph<V>();
            var queue = new BinaryHeap<Edge<V>>();
            var vertices = new HashSet<V>();  // vertices in the tree

            // Adds edges of v pointing out of the tree to the PQ
            void Visit(V v)
            {
                vertices.Add(v);
                foreach (var e in graph.Edges(v))
                {
                    if (!vertices.Contains(e.to))
                        queue.Push(e);  // If e.to is already in the tree then e (or rather its complement) is in the queue
                }
            }

            if (!graph.IsEmpty())
                Visit(graph.Vertices().First());

            while (!queue.IsEmpty())
            {
                var e = queue.Pop();
                if (vertices.Contains(e.to))
                    continue;  // Otherwise we will form a cycle
                tree.Add(e);
                Visit(e.to);
            }
            return tree;
        }
    }

    /// <summary>
    /// Adjacency list representation of a weighted, undirected graph.
    /// </summary>
    public class Graph<V> : IGraph<V>
    {
        // An undirected graph is simply a special case of a directed graph.
        private HashMap<V, HashSet<Edge<V>>> vertices = new HashMap<V, HashSet<Edge<V>>>();

        public IEnumerable<V> Vertices() => vertices.Keys();

        public IEnumerable<Edge<V>> Edges() => vertices.Values().SelectMany(i => i);

        public IEnumerable<Edge<V>> Edges(V vertex) => vertices.Get(vertex);

        public IEnumerable<V> Neighbours(V vertex) => Edges(vertex).Select(e => e.to);

        /// <summary>
        /// Adds this edge to the graph if it is not already present in constant time.
        /// </summary>
        public void Add(Edge<V> edge)
        {
            if (edge == null)
                throw new ArgumentNullException();

            void Add(Edge<V> e)
            {
                var edges = vertices.GetOrElse(e.from, new HashSet<Edge<V>>());
                edges.Add(e);
                vertices[e.from] = edges;
            }
            // When we add v -> w to the adjacency list of v we must also add w -> v to the adjacency list of w
            Add(edge);
            Add(edge.Complement());
        }

        /// <summary>
        /// Adds this vertex to the graph if it is not already present in constant time.
        /// </summary>
        public void Add(V vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException();

            if (!vertices.Contains(vertex))
                vertices.Put(vertex, new HashSet<Edge<V>>());
        }

        /// <summary>
        /// Returns true if the graph contains the given vertex in constant time; false otherwise.
        /// </summary>
        public bool Contains(V vertex) => vertices.Contains(vertex);

        /// <summary>
        /// Returns true if the graph contains the given edge in constant time; false otherwise.
        /// </summary>
        public bool Contains(Edge<V> edge)
        {
            if (edge == null)
                throw new ArgumentNullException();

            var edges = vertices.GetOrElse(edge.from, new HashSet<Edge<V>>());
            return edges.Contains(edge);
        }

        /// <summary>
        /// Removes this vertex from the graph, and any connecting edges, in linear time.
        /// Raises <exception cref="System.InvalidOperationException"> if it is not present.
        /// </summary>
        public void Remove(V vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException();

            // Remove 'incoming' edges to maintain the invariant that v -> w and w -> v exist together
            var incoming = vertices
                .GetOrElse(vertex, new HashSet<Edge<V>>())
                .Select(e => e.Complement());
            foreach (var e in incoming)
                RemoveEdge(e);
            vertices.Delete(vertex);
        }

        /// <summary>
        /// Removes this directed edge <code>edge.from</code> -> <code>edge.to</code> from the graph.
        /// </summary>
        private void RemoveEdge(Edge<V> edge)
        {
            try
            {
                var edges = vertices.Get(edge.from);
                edges.Remove(edge);
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException($"Edge {edge} or its source vertex {edge.from} is not in the graph");
            }
        }

        /// <summary>
        /// Removes this edge from the graph in constant time.
        /// Raises <exception cref="System.InvalidOperationException"> if it is not present.
        /// </summary>
        public void Remove(Edge<V> edge)
        {
            if (edge == null)
                throw new ArgumentNullException();
            RemoveEdge(edge);
            RemoveEdge(edge.Complement());
        }

        public bool IsEmpty() => vertices.IsEmpty();

        /// <summary>
        /// Returns the number of vertices in the graph.
        /// </summary>
        public int Size() => vertices.Size();

        /// <summary>
        /// Returns true if this graph is connected in O(V+E) time; false otherwise. 
        /// </summary>
        public bool IsConnected()
        {
            if (IsEmpty())
                return true;

            // Use DFS to determine whether any vertices are unreachable
            var visited = new HashSet<V>();
            var stack = new LinkedList<V>();
            var s = Vertices().First();
            stack.Push(s);

            while (!stack.IsEmpty())
            {
                var v = stack.Pop();
                visited.Add(v);
                foreach (var w in Neighbours(v))
                {
                    if (visited.Contains(w))
                        continue;
                    stack.Push(w);
                }
            }

            return Vertices().All(visited.Contains);
        }

        /// <summary>
        /// Returns true if this graph is cylic in O(V+E) time; false otherwise.
        /// </summary>
        public bool IsCyclic()
        {
            var visited = new HashSet<V>();
            var nil = None<V>();

            bool IsCyclic(V v, Option<V> parent)
            {
                if (parent.IsEmpty())
                {
                    if (visited.Contains(v))
                        return false;  // Vertex has been visited, but only because it was part of another connected component
                    visited.Add(v);
                    return Neighbours(v).Any(w => IsCyclic(w, Some(v)));
                }
                else
                {
                    if (visited.Contains(v))
                        return true;
                    visited.Add(v);
                    // In an undirected graph going back to the parent leads to a false positive
                    var neighbours = Neighbours(v).Where(w => !parent.Value.Equals(w));
                    return neighbours.Any(w => IsCyclic(w, Some(v)));
                }
            }

            // Run the routine on different vertices as they may belong to different connected components
            return Vertices().Any(v => IsCyclic(v, nil));
        }
    }
}
