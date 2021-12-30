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

        private Node get(String suffix) {
            var get = new BiFunction<Node, Integer, Node>() {
                @Override
                public Node apply(Node node, Integer depth) {
                    if (node == null)
                        return null;  // not found
                    if (depth == suffix.length())
                        return node;  // `node.value` could be `null` if `suffix` is merely a substring of one or more keys
                    return apply(node.children[suffix.charAt(depth)], depth + 1);
                }
            };
            return get.apply(this, 0);
        }

        private Queue<Tuple<String, V>> collect(String suffix) {
            var queue = new Queue<Tuple<String, V>>();
            var sb = new StringBuilder(suffix);
            var root = get(suffix);

            var collect = new Consumer<Node>() {
                @Override
                public void accept(Node node) {
                    if (node == null)
                        return;
                    if (node.value != null)
                        queue.push(new Tuple<>(sb.toString(), node.value));
                    // Performs a DFS traversal across the character set
                    for (char c = 0; c < radix; c++) {
                        sb.append(c);
                        accept(node.children[c]);
                        sb.setLength(sb.length() - 1);
                    }
                }
            };
            collect.accept(root);
            return queue;
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
                // Creates or updates the value of this node
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
        var node = root.get(key);
        if (node == null || node.value == null)
            throw new NoSuchElementException();
        return node.value;
    }

    @Override
    public V getOrElse(String key, V defaultValue) {
        var node = root.get(key);
        if (node == null || node.value == null)
            return defaultValue;
        return node.value;
    }

    @Override
    public void delete(String key) {
        if (key == null)
            throw new IllegalArgumentException();

        var delete = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer depth) {
                if (node == null)
                    throw new NoSuchElementException();
                if (depth == key.length() && node.value == null)
                    throw new NoSuchElementException();

                if (depth == key.length())
                    node.value = null;
                else {
                    var c = key.charAt(depth);
                    node.children[c] = apply(node.children[c], depth + 1);
                }

                // Optimization: Remove non-root nodes if they carry no values
                if (depth == 0 || node.value != null)
                    return node;
                for (var child : node.children)
                    if (child != null)
                        return node;
                return null;
            }
        };

        root = delete.apply(root, 0);
        size--;
    }

    @Override
    public boolean contains(String key) {
        var node = root.get(key);
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
        return root.collect("").iterator();
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
        if (prefix == null)
            throw new IllegalArgumentException();
        return () -> new Iterator<>() {
            private final Iterator<Tuple<String, V>> items = root.collect(prefix).iterator();

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
                // Longest prefix is actually the query, so bail
                break;
            // Try to look for a longer prefix
            depth++;
            node = node.children[query.charAt(depth)];
        }
        return query.substring(0, checkpoint);
    }
}
