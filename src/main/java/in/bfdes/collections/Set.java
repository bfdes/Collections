package in.bfdes.collections;

public interface Set<K> extends Iterable<K> {
    void add(K key);

    boolean contains(K key);

    void remove(K key);

    boolean isEmpty();

    int size();
}
