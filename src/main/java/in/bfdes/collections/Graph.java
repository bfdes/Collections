package in.bfdes.collections;

public interface Graph<V> {
    void add(V vertex);

    void add(Edge<V> edge);

    boolean contains(V vertex);

    boolean contains(Edge<V> edge);

    void remove(V vertex);

    void remove(Edge<V> edge);

    boolean isEmpty();

    int size();

    Iterable<V> vertices();

    Iterable<Edge<V>> edges();

    Iterable<Edge<V>> edges(V vertex);
}
