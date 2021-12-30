package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.BiFunction;
import java.util.function.Consumer;

public class TernarySearchTree<V> implements Trie<V> {
    private Node root;
    private int size;

    private class Node {
        public final char key;
        public V value;
        public Node left;
        public Node middle;
        public Node right;

        public Node(char key) {
            this.key = key;
        }
    }

    @Override
    public void put(String key, V value) {
        if (key == null || value == null)
            throw new IllegalArgumentException();
        var put = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer depth) {
                var c = key.charAt(depth);
                if (node == null)
                    node = new Node(c);
                if (c < node.key)
                    node.left = apply(node.left, depth);
                else if (c > node.key)
                    node.right = apply(node.right, depth);
                else if (depth < key.length() - 1)
                    node.middle = apply(node.middle, depth + 1);
                else if (node.value != null)
                    // `put` is updating an existing key-value pair
                    node.value = value;
                else {
                    // `put` is creating a new key-value pair
                    size++;
                    node.value = value;
                }
                return node;
            }
        };
        root = put.apply(root, 0);
    }

    @Override
    public V get(String key) {
        var node = getNode(key);
        if (node == null || node.value == null)
            throw new NoSuchElementException();
        return node.value;
    }

    @Override
    public V getOrElse(String key, V defaultValue) {
        var node = getNode(key);
        if (node == null || node.value == null)
            return defaultValue;
        return node.value;
    }

    @Override
    public void delete(String key) {
        var delete = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer depth) {
                if (node == null)
                    throw new NoSuchElementException();
                var c = key.charAt(depth);
                if (c < node.key)
                    node.left = apply(node.left, depth);
                else if (c > node.key)
                    node.right = apply(node.right, depth);
                else if (depth < key.length() - 1)
                    node.middle = apply(node.middle, depth + 1);
                else if (node.value == null)
                    throw new NoSuchElementException();
                else
                    node.value = null;
                return node;
            }
        };

        root = delete.apply(root, 0);
        size--;
    }

    @Override
    public boolean contains(String key) {
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
    public Iterator<Tuple<String, V>> iterator() {
        return collect("").iterator();
    }

    @Override
    public Iterable<String> keys() {
        return () -> new Iterator<>() {
            private final Iterator<Tuple<String, V>> items = iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public String next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().first();
            }
        };
    }

    @Override
    public Iterable<V> values() {
        return () -> new Iterator<>() {
            private final Iterator<Tuple<String, V>> items = iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public V next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().second();
            }
        };
    }

    @Override
    public Iterable<String> keys(String prefix) {
        return () -> new Iterator<>() {
            private final Iterator<Tuple<String, V>> items = collect(prefix).iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public String next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().first();
            }
        };
    }

    @Override
    public String longestPrefix(String query) {
        if (query == null)
            throw new IllegalArgumentException();

        var checkpoint = 0;  // represents the length of the longest prefix found
        var depth = 0;
        var node = root;

        while (node != null) {
            if (node.value != null)
                // Found a longer prefix, so save progress
                checkpoint = depth;
            if (depth == query.length())
                // Longest prefix is actually the whole query, so bail now
                break;

            // Try to look for a longer prefix
            var c = query.charAt(depth);
            if (c < node.key)
                node = node.left;
            else if (c > node.key)
                node = node.right;
            else {
                depth++;
                node = node.middle;
            }
        }
        return query.substring(0, checkpoint);
    }

    private Node getNode(String suffix) {
        if (suffix == null)
            throw new IllegalArgumentException();
        var get = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer depth) {
                if (node == null)
                    return null; // not found
                var c = suffix.charAt(depth);
                if (c < node.key)
                    return apply(node.left, depth);
                else if (c > node.key)
                    return apply(node.right, depth);
                else if (depth < suffix.length() - 1)
                    return apply(node.middle, depth + 1);  // character match
                return node; // `node.value` could be `null` if `suffix` is merely a substring of one or more keys
            }
        };
        return get.apply(root, 0);
    }

    private Queue<Tuple<String, V>> collect(String suffix) {
        var queue = new Queue<Tuple<String, V>>();
        var sb = new StringBuilder(suffix);
        var root = getNode(suffix);

        var collect = new Consumer<Node>() {
            @Override
            public void accept(Node node) {
                if (node == null)
                    return;
                accept(node.left);
                sb.append(node.key);
                if (node.value != null)
                    queue.push(new Tuple<>(sb.toString(), node.value));
                accept(node.middle);
                sb.setLength(sb.length() - 1);
                accept(node.right);
            }
        };

        collect.accept(root);
        return queue;
    }
}
