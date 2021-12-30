package in.bfdes.collections;

public interface PriorityQueue<K extends Comparable<K>> {
    void push(K key);

    K peek();

    K pop();

    int size();

    boolean isEmpty();
}
