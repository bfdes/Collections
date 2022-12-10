package in.bfdes.collections;

public interface Graph<V> {
    /**
     * Adds a vertex to the graph, if it does not already exist.
     */
    void add(V vertex);

    void add(Edge<V> edge);

    boolean contains(V vertex);

    boolean contains(Edge<V> edge);

    void remove(V vertex);

    void remove(Edge<V> edge);

    /**
     * Returns true if this graph contains no vertices, false otherwise.
     */
    boolean isEmpty();

    /**
     * Returns the number of vertices in this graph.
     */
    int size();

    Iterable<V> vertices();

    Iterable<Edge<V>> edges();

    Iterable<Edge<V>> edges(V vertex);
}
