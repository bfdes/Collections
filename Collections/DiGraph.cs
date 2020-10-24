using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    public static class DiGraph
    {
        /// <summary>
        /// Returns all the vertices of this graph in topological order in O(V+E) time.
        /// Raises <exception cref="System.InvalidOperationException"> if the graph is cyclic.
        /// </summary>
        public static IEnumerable<V> TopologicalSort<V>(DiGraph<V> graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            // A digraph containing a directed cycle cannot be topologically ordered
            if (graph.IsCyclic())
                throw new InvalidOperationException();

            // In topological order v preceedes w for an edge v -> w.
            // The reversed output of a postorder DFS trace can be useds to perform topological sorting of vertices.

            return ReversePostOrder(graph);
        }

        private static IEnumerable<V> ReversePostOrder<V>(DiGraph<V> graph)
        {
            var visited = new HashSet<V>();
            var stack = new LinkedList<V>();

            void Go(V v)
            {
                visited.Add(v);
                var neighbours = graph.Edges(v).Select(e => e.to);
                foreach (var w in neighbours)
                {
                    if (!visited.Contains(w))
                        Go(w);
                }
                stack.Push(v);
            }

            // Account for the fact that:
            // - Not all vertices are reachable from one another
            // - The vertex we call Go on first might have incoming links
            foreach (var v in graph.Vertices())
            {
                if (!visited.Contains(v))
                    Go(v);
            }

            return stack;
        }

        /// <summary>
        /// Returns all the strongly connected components that comprise this <c>graph</c> in O(V+E) time.
        /// </summary>
        /// <returns>A set containing each strongly connected component, represented as a subgraph</returns>
        public static ISet<DiGraph<V>> Components<V>(DiGraph<V> graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            // Kosaraju-Sharir algorithm
            // 1. Return vertices of G^R in reverse postorder
            // 2. Run DFS on G, visiting unmarked vertices in the order above

            // 1.
            var reversed = ReversePostOrder(graph.Complement());

            // 2.
            var visited = new HashSet<V>();

            // Creates and returns a subgraph containing vertices reachable by s only
            DiGraph<V> Component(V s)
            {
                var g = new DiGraph<V>();
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

            var components = new HashSet<DiGraph<V>>();

            foreach (var v in reversed)
            {
                if (visited.Contains(v))
                    continue;  // v is part of a component that has already been created
                components.Add(Component(v));
            }

            return components;
        }

        /// <summary>
        /// Solves the single-source shortest path problem in O(E*ln V) time for a graph with none-negative edge weights. 
        /// </summary>
        public static Paths<V> Dijkstra<V>(DiGraph<V> graph, V source) where V : class
        {
            if (graph == null || source == null)
                throw new ArgumentNullException();

            if (!graph.Contains(source))
                throw new ArgumentException();

            if (graph.Edges().Any(e => e.weight < 0))
                throw new ArgumentException();

            var queue = new BinaryHeap<Double, V>();
            var distTo = new HashMap<V, Double>();
            var edgeTo = new HashMap<V, Edge<V>>();

            void Relax(Edge<V> e)
            {
                var (v, w) = (e.from, e.to);
                var weight = new Double(e.weight);
                if (distTo[v] > distTo[w] + weight)
                {
                    distTo[w] = distTo[v] + weight;
                    edgeTo[w] = e;
                    if (queue.Contains(w))
                        queue.Update(w, distTo[w]);
                    else
                        queue.Push(w, distTo[w]);
                }
            }

            foreach (var v in graph.Vertices())
                distTo[v] = Double.MaxValue;
            distTo[source] = new Double(0);

            queue.Push(source, new Double(0));

            while (!queue.IsEmpty())
            {
                var (_, v) = queue.Pop();
                foreach (var e in graph.Edges(v))
                    Relax(e);
            }
            return new Paths<V>(source, edgeTo);
        }

        /// <summary>
        /// Solves the single-source shortest path problem in O(E*V) time for a graph without any negative cycles, or in O(V+E) time if it is a DAG.
        /// </summary>
        public static Paths<V> BellmanFord<V>(DiGraph<V> graph, V source)
        {
            if (graph == null || source == null)
                throw new ArgumentNullException();

            if (!graph.Contains(source))
                throw new ArgumentException();

            var distTo = new HashMap<V, Double>();
            var edgeTo = new HashMap<V, Edge<V>>();

            bool Relax(Edge<V> e)
            {
                var (v, w) = (e.from, e.to);
                var weight = new Double(e.weight);
                if (distTo[v] > distTo[w] + weight)
                {
                    distTo[w] = distTo[v] + weight;
                    edgeTo[w] = e;
                    return true;
                }
                return false;
            }

            foreach (var v in graph.Vertices())
                distTo[v] = Double.MaxValue;
            distTo[source] = new Double(0);

            // The Bellman Ford procedure involves relaxing every edge V times.
            // However, for a DAG we can delegate to a simpler algorithm that runs in linear time

            try
            {
                var vertices = TopologicalSort(graph);
                foreach (var v in vertices)
                    foreach (var e in graph.Edges(v))
                        Relax(e);
            }
            catch (InvalidOperationException)
            {
                // Graph is not a DAG
                var N = graph.Size();
                foreach (var i in Enumerable.Range(1, N))
                {
                    foreach (var e in graph.Edges())
                    {
                        var relaxed = Relax(e);
                        if (i == N && relaxed)
                            throw new InvalidOperationException("Negative cycle detected");
                    }
                }

            }

            return new Paths<V>(source, edgeTo);
        }
    }

    /// <summary>
    /// Adjacency list representation of a weighted, directed graph.
    /// </summary>
    public class DiGraph<V> : IGraph<V>
    {
        // For sparse graphs an adjacency list representation is more efficient than using a matrix
        private HashMap<V, HashSet<Edge<V>>> vertices = new HashMap<V, HashSet<Edge<V>>>();

        public IEnumerable<V> Vertices() => vertices.Keys();

        public IEnumerable<Edge<V>> Edges() => vertices.Values().SelectMany(i => i);

        /// <summary>
        /// Returns all the edges with the vertex <code>v</code> as their source.
        /// </summary>
        public IEnumerable<Edge<V>> Edges(V vertex) => vertices.Get(vertex);

        public DiGraph<V> Complement()
        {
            var complement = new DiGraph<V>();
            foreach (var e in Edges())
                complement.Add(e.Complement());
            return complement;
        }

        public void Add(Edge<V> edge)
        {
            if (edge == null)
                throw new ArgumentNullException();

            var edges = vertices.GetOrElse(edge.from, new HashSet<Edge<V>>());
            edges.Add(edge);
            vertices[edge.from] = edges;
        }

        public void Add(V vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException();

            if (!vertices.Contains(vertex))
                vertices.Put(vertex, new HashSet<Edge<V>>());
        }

        public bool Contains(V vertex) => vertices.Contains(vertex);

        public bool Contains(Edge<V> edge)
        {
            if (edge == null)
                throw new ArgumentNullException();

            var edges = vertices.GetOrElse(edge.from, new HashSet<Edge<V>>());
            return edges.Contains(edge);
        }

        public void Remove(V vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException();

            var incoming = Edges().Where(e => e.to.Equals(vertex));
            foreach (var e in incoming)
                Remove(e);
            vertices.Delete(vertex);  // Outgoing edges also removed
        }

        public void Remove(Edge<V> edge)
        {
            if (edge == null)
                throw new ArgumentNullException();

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

        public bool IsEmpty() => vertices.IsEmpty();

        public int Size() => vertices.Size();

        /// <summary>
        /// Returns true if this graph is cyclic in O(V+E) time, false otherwise.
        /// </summary>
        public bool IsCyclic()
        {
            // We use the recursive form of DFS to determine if a graph is cyclic
            var visited = new HashSet<V>();
            var recursionStack = new HashSet<V>();

            bool IsCyclic(V v)
            {
                // If we encounter the same node within a DFS trace then the graph must be cyclic
                if (recursionStack.Contains(v))
                    return true;
                if (visited.Contains(v))
                    return false;
                visited.Add(v);
                recursionStack.Add(v);

                var isCyclic = Edges(v).Select(e => e.to).Any(IsCyclic);
                recursionStack.Remove(v);
                return isCyclic;
            }

            return Vertices().Any(IsCyclic);  // Run the routine on every vertex as vertices may not all be reachable from one another
        }
    }
}
