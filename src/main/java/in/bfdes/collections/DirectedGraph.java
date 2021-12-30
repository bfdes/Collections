package in.bfdes.collections;

import java.util.function.Predicate;

/**
 * Adjacency list representation of a weighted, directed graph.
 */
public class DirectedGraph<V> implements Graph<V> {
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
        // Adding `edge` involves adding `edge.from` and `edge.to` to the graph, if they are not already present
        var edges = vertices.getOrElse(edge.from(), new HashSet<>());
        edges.add(edge);
        vertices.put(edge.from(), edges);
        add(edge.to());
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
        // Remove both incoming and outgoing edges for `vertex`, which requires a full scan
        for (var edge : edges())
            if (edge.to().equals(vertex))
                remove(edge); // Note: `edge.from` may be left without any edges
        vertices.delete(vertex);
    }

    @Override
    public void remove(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();
        var edges = vertices.get(edge.from());
        edges.remove(edge);  // Note: `edge.from` and `edge.to` may be left without any edges
    }

    @Override
    public boolean isEmpty() {
        return vertices.isEmpty();
    }

    @Override
    public int size() {
        return vertices.size();
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

    /**
     * Returns the outgoing edges of a vertex.
     */
    @Override
    public Iterable<Edge<V>> edges(V vertex) {
        return vertices.get(vertex);
    }

    public Iterable<V> neighbours(V vertex) {
        // TODO Stream vertices without copying all of them first
        var neighbours = new LinkedList<V>();
        for (var edge : edges(vertex))
            neighbours.push(edge.to());
        return neighbours;
    }

    public DirectedGraph<V> complement() {
        var graph = new DirectedGraph<V>();
        for (var edge : edges())
            graph.add(edge.complement());
        return graph;
    }

    /**
     * Returns `true` if this graph is cyclic in O(V+E) time, `false` otherwise.
     */
    public boolean isCyclic() {
        // Use recursive DFS to determine whether a graph is cyclic.
        // In a depth-first trace of a cyclic graph at least one vertex will appear twice.
        var cache = new HashSet<V>();
        var stack = new HashSet<V>();

        // Alternatively v -> flag  -1 unvisited, 0 visited and in stack, 1 visited and popped off stack

        var isCyclic = new Predicate<V>() {
            @Override
            public boolean test(V vertex) {
                if (stack.contains(vertex))
                    return true;  // `vertex` encountered twice in a depth-first trace
                if (cache.contains(vertex))
                    return false;  // `vertex` was processed in some other depth-first trace
                cache.add(vertex);
                stack.add(vertex);

                for (var neighbour : neighbours(vertex))
                    if (test(neighbour))
                        return true;
                stack.remove(vertex);
                return false;
            }
        };

        // Run the routine on every vertex as vertices may not all be reachable from one another
        for (var vertex : vertices())
            if (isCyclic.test(vertex))
                return true;
        return false;
    }
}
