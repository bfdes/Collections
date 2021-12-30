package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.Function;

/**
 * Associative array abstraction that supports constant time access to key-value pairs.
 * Uses the separate chaining strategy to account for key hash collisions.
 */
public class HashMap<K, V> implements Map<K, V> {
    private int size = 0;  // the number of key-value pairs in the table
    private Node[] table;

    private class Node {
        public final K key;
        public V value;
        public Node next;

        public Node(K key, V value, Node next) {
            this.key = key;
            this.value = value;
            this.next = next;
        }
    }

    public HashMap() {
        this(4);
    }

    public HashMap(int capacity) {
        if (capacity < 1)
            throw new IllegalArgumentException();
        table = (Node[]) new Object[capacity];
    }

    @Override
    public void put(K key, V value) {
        if (key == null)
            throw new IllegalArgumentException();

        // Double the table size if the average length of a chain is >= 10
        if (size >= 10 * table.length) resize(2 * table.length);

        var i = hash(key);
        var node = table[i];

        while (node != null) {
            if (key.equals(node.key)) {
                node.value = value;
                return;
            }
            node = node.next;
        }
        // Not found
        size++;
        table[i] = new Node(key, value, table[i]);
    }

    @Override
    public V get(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var i = hash(key);
        var node = table[i];

        while (node != null) {
            if (key.equals(node.key))
                return node.value;
            node = node.next;
        }

        throw new NoSuchElementException();
    }

    @Override
    public V getOrElse(K key, V defaultValue) {
        if (key == null)
            throw new IllegalArgumentException();

        var i = hash(key);
        var node = table[i];

        while (node != null) {
            if (key.equals(node.key))
                return node.value;
            node = node.next;
        }

        return defaultValue;
    }

    @Override
    public void delete(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var delete = new Function<Node, Node>() {
            @Override
            public Node apply(Node node) {
                if (node == null)
                    throw new IllegalArgumentException();
                if (key.equals(node.key)) {
                    size--;
                    return node.next;
                }
                node.next = apply(node.next);
                return node;
            }
        };

        var i = hash(key);
        table[i] = delete.apply(table[i]);

        // Halve the table size if the average length of a chain is <= 2
        if (size > 4 && size <= 2 * table.length) resize(size / 2);
    }

    @Override
    public boolean contains(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var i = hash(key);
        var node = table[i];

        while (node != null) {
            if (key.equals(node.key))
                return true;
            node = node.next;
        }
        return false;
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
    public Iterator<Tuple<K, V>> iterator() {
        return new Iterator<>() {
            private int nextChain = 0;
            private Node nextNode;

            @Override
            public boolean hasNext() {
                return nextNode != null || nextChain < table.length - 1;
            }

            @Override
            public Tuple<K, V> next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                if (nextNode == null)
                    nextNode = table[nextChain++];
                var pair = new Tuple<>(nextNode.key, nextNode.value);
                nextNode = nextNode.next;
                return pair;
            }
        };
    }

    @Override
    public Iterable<K> keys() {
        return () -> new Iterator<>() {
            private final Iterator<Tuple<K, V>> items = iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public K next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().first();
            }
        };
    }

    @Override
    public Iterable<V> values() {
        return () -> new Iterator<>() {
            private final Iterator<Tuple<K, V>> items = iterator();

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

    private void resize(int newCapacity) {
        var temp = new HashMap<K, V>(newCapacity);
        for (var pair : this)
            temp.put(pair.first(), pair.second());
        table = temp.table;
    }

    private int hash(K key) {
        return (key.hashCode() & 0x7fffffff) % table.length;
    }
}
