package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.BiFunction;
import java.util.function.Consumer;

public class TernarySearchTree<V> implements Trie<V> {
    private Node root;
    private int size;

    private class Node {
        public final char c;
        public V value;
        public Node left;
        public Node middle;
        public Node right;

        public Node(char c) {
            this.c = c;
        }
    }

    @Override
    public void put(String key, V value) {
        if (key == null || value == null)
            throw new IllegalArgumentException();
        if (key.length() == 0)
            throw new IllegalArgumentException();

        var put = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer depth) {
                var c = key.charAt(depth);
                if (node == null)
                    node = new Node(c);
                if (c < node.c)
                    node.left = apply(node.left, depth);
                else if (c > node.c)
                    node.right = apply(node.right, depth);
                else if (depth < key.length() - 1)
                    node.middle = apply(node.middle, depth + 1);
                else if (node.value == null) {
                    size++;
                    node.value = value;
                } else
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
        if (key.length() == 0)
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
        if (key.length() == 0)
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
        if (key.length() == 0)
            throw new IllegalArgumentException();

        var node = root;
        var depth = 0;

        while (node != null && depth < key.length()) {
            var c = key.charAt(depth);
            if (c < node.c)
                node = node.left;
            else if (c > node.c)
                node = node.right;
            else {
                node = node.middle;
                depth++;
            }
        }

        if (node == null || node.value == null)
            return;
        size--;
        node.value = null;
    }

    @Override
    public boolean contains(String key) {
        if (key == null)
            throw new IllegalArgumentException();
        if (key.length() == 0)
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
    public java.lang.Iterable<V> values() {
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

        while (node != null && depth < query.length()) {
            var c = query.charAt(depth);
            if (c < node.c)
                node = node.left;
            else if (c > node.c)
                node = node.right;
            else if (node.value == null) {
                depth++;
                node = node.middle;
            } else {
                length = ++depth;
                node = node.middle;
            }
        }

        return query.substring(0, length);
    }

    private Node getNode(String prefix) {
        var node = root;
        var depth = 0;

        while (node != null && depth < prefix.length()) {
            var c = prefix.charAt(depth);
            if (c < node.c)
                node = node.left;
            else if (c > node.c)
                node = node.right;
            else {
                node = node.middle;
                depth++;
            }
        }

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
                accept(node.left);
                sb.append(node.c);
                if (node.value != null)
                    queue.push(new Pair<>(sb.toString(), node.value));
                accept(node.middle);
                sb.setLength(sb.length() - 1);
                accept(node.right);
            }
        };

        collect.accept(node);

        return queue;
    }
}
