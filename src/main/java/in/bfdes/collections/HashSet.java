package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.Function;

public class HashSet<K> implements Set<K> {
    private int size = 0;  // the number of key-value pairs in the table
    private Node[] table;

    private class Node {
        public final K key;
        public Node next;

        public Node(K key, Node next) {
            this.key = key;
            this.next = next;
        }
    }

    public HashSet() {
        this(4);
    }

    public HashSet(int capacity) {
        if (capacity < 1)
            throw new IllegalArgumentException();
        table = (Node[]) new Object[capacity];
    }

    @Override
    public void add(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        // Double the table size if the average length of a chain is >= 10
        if (size >= 10 * table.length) resize(2 * table.length);

        var i = hash(key);
        var node = table[i];

        while (node != null) {
            if (key.equals(node.key))
                return;
            node = node.next;
        }
        // Not found
        size++;
        table[i] = new Node(key, table[i]);
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
    public void remove(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var delete = new Function<Node, Node>() {
            @Override
            public Node apply(Node node) {
                if (node == null)
                    return null;  // Not found
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
    public boolean isEmpty() {
        return size == 0;
    }

    @Override
    public int size() {
        return size;
    }

    @Override
    public Iterator<K> iterator() {
        return new Iterator<>() {
            private int nextChain = 0;
            private Node nextNode;

            @Override
            public boolean hasNext() {
                return nextNode != null || nextChain < table.length - 1;
            }

            @Override
            public K next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                if (nextNode == null)
                    nextNode = table[nextChain++];
                var key = nextNode.key;
                nextNode = nextNode.next;
                return key;
            }
        };
    }

    private void resize(int newCapacity) {
        var temp = new HashSet<K>(newCapacity);
        for (var key : this)
            temp.add(key);
        table = temp.table;
    }

    private int hash(K key) {
        return (key.hashCode() & 0x7fffffff) % table.length;
    }
}
