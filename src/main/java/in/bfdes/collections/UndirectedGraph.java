package in.bfdes.collections;

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
        edges.add(edge.complement());
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
            edges.remove(edge.complement());
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
        edges.remove(edge.complement());
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

    /**
     * Returns `true` if this graph is undirected in O(E) time, `false` otherwise.
     * Note: An `UndirectedGraph` should always satisfy this invariant -- this method is for debugging.
     */
    public boolean isUndirected() {
        for (var edge : edges())
            if (!contains(edge.complement()))
                return false;
        return true;
    }

    /**
     * Returns `true` if this graph is connected in O(V+E) time, `false` otherwise.
     */
    public boolean isConnected() {
        // Two vertices u, v are connected if there exists a path from u to v.
        // A graph is connected if every pair of its vertices are connected.
        // An undirected graph is connected if any one vertex, the "start vertex", can reach every other vertex.

        var iterator = vertices().iterator();

        if (!iterator.hasNext())
            return true;  // an empty graph is a connected graph

        var startVertex = iterator.next();
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

        for (var vertex : vertices())
            if (!component.contains(vertex))
                return false;  // at least one vertex is not part of the connected component
        return true;
    }

    /**
     * Returns `true` if this graph is cyclic in O(V+E) time, `false` otherwise.
     */
    public boolean isCyclic() {
        throw new RuntimeException("Not implemented");  // TODO
    }
}
