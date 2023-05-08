package in.bfdes.collections;

public interface PriorityQueue<V, K extends Comparable<K>> {
    boolean contains(V value);

    void push(V value, K key);

    V peek();

    V pop();

    int size();

    boolean isEmpty();
}
