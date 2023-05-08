package in.bfdes.collections;

public interface Map<K, V> extends Iterable<Pair<K, V>> {
    void put(K key, V value);

    V get(K key);

    V getOrElse(K key, V defaultValue);

    void delete(K key);

    boolean contains(K key);

    boolean isEmpty();

    int size();

    Iterable<K> keys();

    Iterable<V> values();
}
