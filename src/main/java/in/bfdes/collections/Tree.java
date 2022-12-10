package in.bfdes.collections;

public interface Tree<K extends Comparable<K>, V> extends Map<K, V> {
    K min();

    K max();

    K floor(K key);

    K ceiling(K key);

    int rank(K key);
}
