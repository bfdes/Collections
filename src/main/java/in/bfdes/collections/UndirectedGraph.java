package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.BiPredicate;
import java.util.function.Function;

/**
 * Adjacency list representation of a weighted, undirected graph.
 * <p>
 * An undirected graph is implemented as a directed graph which maintains the following invariant:
 * "If an edge (from, to) is a member of the graph, then so is its complement (to, from)."
 */
public class UndirectedGraph<V> implements Graph<V> {
    private final Map<V, Set<Edge<V>>> vertices = new HashMap<>();

    @Override
    public void add(V vertex) {
        if (!contains(vertex))
            vertices.put(vertex, new HashSet<>());
    }

    @Override
    public void add(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();

        // Add `edge` to the graph, as well as `edge.from` (if it is not already present)
        var edges = vertices.getOrElse(edge.from(), new HashSet<>());
        edges.add(edge);
        vertices.put(edge.from(), edges);

        // Add `edge.complement` to the graph, as well as `edge.to` (if it is not already present)
        edges = vertices.getOrElse(edge.to(), new HashSet<>());
        edges.add(edge.transpose());
        vertices.put(edge.to(), edges);
    }

    @Override
    public boolean contains(V vertex) {
        if (vertex == null)
            throw new IllegalArgumentException();
        return vertices.contains(vertex);
    }

    @Override
    public boolean contains(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();

        var edges = vertices.getOrElse(edge.from(), new HashSet<>());
        return edges.contains(edge);
    }

    @Override
    public void remove(V vertex) {
        if (vertex == null)
            throw new IllegalArgumentException();
        // Remove any edges this vertex is part of:
        // 1) Remove the "incoming" edges of `vertex`
        for (var edge : edges(vertex)) {
            var edges = vertices.get(edge.to());
            edges.remove(edge.transpose());
        }
        // 2) Remove the "outgoing" edges of `vertex`
        vertices.delete(vertex);
    }

    @Override
    public void remove(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();
        // Remove `edge`,
        var edges = vertices.get(edge.from());
        edges.remove(edge);

        // , as well as its mirror, `edge.complement`
        edges = vertices.get(edge.to());
        edges.remove(edge.transpose());
    }

    @Override
    public boolean isEmpty() {
        return vertices.isEmpty();
    }

    @Override
    public int size() {
        return vertices.size() / 2;
    }

    @Override
    public Iterable<V> vertices() {
        return vertices.keys();
    }

    @Override
    public Iterable<Edge<V>> edges() {
        // TODO Stream edges without copying all of them first
        var edges = new LinkedList<Edge<V>>();
        for (var vertex : vertices())
            for (var edge : edges(vertex))
                edges.push(edge);
        return edges;
    }

    @Override
    public Iterable<Edge<V>> edges(V vertex) {
        return vertices.get(vertex);
    }

    public Iterable<V> neighbours(V vertex) {
        return () -> new Iterator<>() {
            private final Iterator<Edge<V>> edges = edges(vertex).iterator();

            @Override
            public boolean hasNext() {
                return edges.hasNext();
            }

            @Override
            public V next() {
                if (!edges.hasNext())
                    throw new NoSuchElementException();
                return edges.next().to();
            }
        };
    }

    /**
     * Returns `true` if this graph is connected in O(V+E) time, `false` otherwise.
     */
    public boolean isConnected() {
        // Two vertices u, v are connected if there exists a path from u to v.
        // A graph is connected if every pair of its vertices are connected.
        // An undirected graph is connected if any vertex can reach every other vertex.

        var vertices = vertices().iterator();

        if (!vertices.hasNext())
            return true;  // an empty graph is a connected graph

        var startVertex = vertices.next();
        var component = new HashSet<V>();
        var stack = new LinkedList<V>();
        stack.push(startVertex);

        while (!stack.isEmpty()) {
            var vertex = stack.pop();
            component.add(vertex);
            for (var neighbour : neighbours(vertex))
                if (!component.contains(neighbour))
                    stack.push(neighbour);
        }

        while (vertices.hasNext())
            if (!component.contains(vertices.next()))
                return false;  // at least one vertex is not part of the connected component
        return true;
    }

    /**
     * Returns `true` if this graph is cyclic in O(V+E) time, `false` otherwise.
     */
    public boolean isCyclic() {
        var visitedVertices = new HashSet<V>();

        var isCyclic = new BiPredicate<V, V>() {
            public boolean test(V vertex) {
                if (visitedVertices.contains(vertex))
                    return false; // `vertex` has been visited, but only because it was part of some other component
                visitedVertices.add(vertex);
                for (var neighbour : neighbours(vertex))
                    if (test(neighbour, vertex))
                        return true;
                return false;
            }

            @Override
            public boolean test(V vertex, V parent) {
                if (visitedVertices.contains(vertex))
                    return true;  // encountered `vertex` twice in a DFS trace
                visitedVertices.add(vertex);
                for (var neighbour : neighbours(vertex))
                    // Continue the DFS trace, but do not go back to `parent` (leads to false positive)
                    if (!parent.equals(neighbour) && test(neighbour, vertex))
                        return true;
                return false;
            }
        };

        for (var vertex : vertices())
            if (isCyclic.test(vertex))
                return true;
        return false;
    }

    /**
     * Returns all the connected components that comprise this graph in O(V+E) time.
     *
     * @return A set containing each connected component, represented as a subgraph
     */
    public Iterable<UndirectedGraph<V>> components() {
        var components = new LinkedList<UndirectedGraph<V>>();
        var visitedVertices = new HashSet<V>();

        // Creates and returns a subgraph containing vertices reachable by `source` only
        var component = new Function<V, UndirectedGraph<V>>() {
            @Override
            public UndirectedGraph<V> apply(V source) {
                var component = new UndirectedGraph<V>();
                var stack = new LinkedList<V>();
                stack.push(source);

                while (!stack.isEmpty()) {
                    var vertex = stack.pop();
                    visitedVertices.add(vertex);
                    for (var edge : edges(vertex)) {
                        var neighbour = edge.to();
                        if (visitedVertices.contains(neighbour))
                            continue;  // n.b. `neighbour` must be part of this particular component, `component`
                        component.add(edge);
                        stack.push(neighbour);
                    }
                }
                return component;
            }
        };

        for (var vertex : vertices())
            if (!visitedVertices.contains(vertex))
                components.push(component.apply(vertex));
        return components;
    }
}
