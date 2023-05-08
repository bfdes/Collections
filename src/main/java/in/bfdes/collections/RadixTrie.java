package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.BiFunction;
import java.util.function.Consumer;

public class RadixTrie<V> implements Trie<V> {
    public static final int radix = 256;  // support for ASCII chars
    private Node root = new Node();
    private int size = 0;

    private class Node {
        public V value;
        public Node[] children = (Node[]) new Object[radix];

        public Node(V value) {
            this.value = value;
        }

        public Node() {
        }
    }

    @Override
    public void put(String key, V value) {
        if (key == null || value == null)
            throw new IllegalArgumentException();

        var put = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer depth) {
                if (node == null)
                    node = new Node();
                if (depth < key.length()) {
                    var c = key.charAt(depth);
                    node.children[c] = apply(node.children[c], depth + 1);
                    return node;
                }
                if (node.value == null)
                    size++;
                node.value = value;
                return node;
            }
        };

        root = put.apply(root, 0);
    }

    @Override
    public V get(String key) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = getNode(key);
        if (node == null || node.value == null)
            throw new NoSuchElementException();
        return node.value;
    }

    @Override
    public V getOrElse(String key, V defaultValue) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = getNode(key);
        if (node == null || node.value == null)
            return defaultValue;
        return node.value;
    }

    @Override
    public void delete(String key) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = root;
        var depth = 0;

        while (node != null && depth < key.length())
            node = node.children[key.charAt(depth++)];

        if (node == null || node.value == null)
            return;
        size--;
        node.value = null;
    }

    @Override
    public boolean contains(String key) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = getNode(key);
        return node != null && node.value != null;
    }

    @Override
    public boolean isEmpty() {
        return size == 0;
    }

    @Override
    public int size() {
        return size;
    }

    @Override
    public Iterator<Pair<String, V>> iterator() {
        return items("").iterator();
    }

    @Override
    public Iterable<String> keys() {
        return keys("");
    }

    @Override
    public Iterable<V> values() {
        return () -> new Iterator<>() {
            private final Iterator<Pair<String, V>> items = iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public V next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().value();
            }
        };
    }

    @Override
    public Iterable<String> keys(String prefix) {
        if (prefix == null)
            throw new IllegalArgumentException();

        return () -> new Iterator<>() {
            private final Iterator<Pair<String, V>> items = items(prefix).iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public String next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().key();
            }
        };
    }

    @Override
    public String longestPrefix(String query) {
        if (query == null)
            throw new IllegalArgumentException();

        var length = 0;
        var depth = 0;
        var node = root;

        while (node != null) {
            if (node.value != null)
                length = depth;
            if (depth == query.length())
                break;
            node = node.children[query.charAt(depth++)];
        }
        return query.substring(0, length);
    }

    private Node getNode(String prefix) {
        var node = root;
        var depth = 0;

        while (node != null && depth < prefix.length())
            node = node.children[prefix.charAt(depth++)];

        return node;
    }

    private Iterable<Pair<String, V>> items(String prefix) {
        var queue = new Queue<Pair<String, V>>();
        var sb = new StringBuilder(prefix);
        var node = getNode(prefix);

        var collect = new Consumer<Node>() {
            @Override
            public void accept(Node node) {
                if (node == null)
                    return;
                if (node.value != null)
                    queue.push(new Pair<>(sb.toString(), node.value));
                for (char c = 0; c < radix; c++) {
                    sb.append(c);
                    accept(node.children[c]);
                    sb.setLength(sb.length() - 1);
                }
            }
        };

        collect.accept(node);

        return queue;
    }
}
