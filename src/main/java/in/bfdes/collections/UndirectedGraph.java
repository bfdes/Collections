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
public class UndirectedGraph<V> extends DirectedGraph<V> {
    private final Map<V, Set<Edge<V>>> vertices = new HashMap<>();

    @Override
    public void add(Edge<V> edge) {
        super.add(edge);
        super.add(edge.transpose());
    }

    @Override
    public void remove(V vertex) {
        if (vertex == null)
            throw new IllegalArgumentException();
        for (var edge : edges(vertex))
            remove(edge);
        vertices.delete(vertex);
    }

    @Override
    public void remove(Edge<V> edge) {
        super.remove(edge);
        super.remove(edge.transpose());
    }

    @Override
    public int size() {
        return super.size() / 2;
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
