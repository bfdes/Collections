package in.bfdes.collections;

public interface Trie<V> extends Map<String, V> {
    Iterable<String> keys(String prefix);

    String longestPrefix(String query);
}
