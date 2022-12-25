package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class LinkedList<T> implements List<T> {
    private Node<T> head;
    private int size = 0;

    private record Node<U>(U item, Node<U> next) {
    }

    public LinkedList() {
    }

    public LinkedList(Iterable<T> initialItems) {
        if (initialItems == null)
            throw new IllegalArgumentException();
        for (var item : initialItems)
            push(item);
    }

    public LinkedList(T[] initialItems) {
        if (initialItems == null)
            throw new IllegalArgumentException();
        for (var item : initialItems)
            push(item);
    }

    @Override
    public int size() {
        return size;
    }

    @Override
    public boolean isEmpty() {
        return size == 0;
    }

    @Override
    public T peek() {
        if (isEmpty())
            throw new NoSuchElementException();
        return head.item;
    }

    @Override
    public T pop() {
        if (isEmpty())
            throw new NoSuchElementException();
        var item = head.item;
        head = head.next;
        size--;
        return item;
    }

    @Override
    public void push(T item) {
        if (item == null)
            throw new IllegalArgumentException();
        head = new Node<>(item, head);
        size++;
    }

    @Override
    public Iterator<T> iterator() {
        return new Iterator<>() {
            private Node<T> next = head;

            @Override
            public boolean hasNext() {
                return next != null;
            }

            @Override
            public T next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                var item = next.item;
                next = next.next;
                return item;
            }
        };
    }
}
