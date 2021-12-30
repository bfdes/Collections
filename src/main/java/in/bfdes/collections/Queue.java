package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class Queue<T> implements Iterable<T> {
    private Node head;
    private Node tail;
    private int size = 0;

    private class Node {
        public final T item;
        public Node next;

        public Node(T item, Node next) {
            this.item = item;
            this.next = next;
        }
    }

    public Queue() {
    }

    public Queue(T[] initialItems) {
        for (var item : initialItems)
            push(item);
    }

    public Queue(Iterable<T> initialItems) {
        if (initialItems == null)
            throw new IllegalArgumentException();
        for (var item : initialItems)
            push(item);
    }

    public int size() {
        return size;
    }

    public boolean isEmpty() {
        return size == 0;
    }

    public T peek() {
        if (isEmpty())
            throw new NoSuchElementException();
        return head.item;
    }

    public T pop() {
        if (isEmpty())
            throw new NoSuchElementException();
        var item = head.item;
        head = head.next;
        size--;
        return item;
    }

    public void push(T item) {
        if (item == null)
            throw new IllegalArgumentException();
        var last = new Node(item, null);
        if (isEmpty()) {
            head = last;
        } else {
            tail.next = last;
        }
        tail = last;
        size++;
    }

    public T dequeue() {
        return pop();
    }

    public void enqueue(T item) {
        push(item);
    }

    @Override
    public Iterator<T> iterator() {
        return new Iterator<>() {
            private Node next = head;

            public boolean hasNext() {
                return next != null;
            }

            public T next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                T item = next.item;
                next = next.next;
                return item;
            }
        };
    }
}
