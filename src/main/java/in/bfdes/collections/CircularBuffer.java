package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class CircularBuffer<T> implements Iterable<T> {
    private final T[] buffer;
    private int head = 0;
    private int tail = 0;
    private int size = 0;

    public CircularBuffer(int capacity) {
        if (capacity < 1)
            throw new IllegalArgumentException();
        buffer = (T[]) new Object[capacity];
    }

    public CircularBuffer(T[] items) {
        this(Math.max(1, items.length));
        for (var item : items)
            push(item);
    }

    public int size() {
        return size;
    }

    public int capacity() {
        return buffer.length;
    }

    public boolean isEmpty() {
        return size == 0;
    }

    public boolean isFull() {
        return size == buffer.length;
    }

    public T peek() {
        if (isEmpty())
            throw new NoSuchElementException();
        return buffer[head];
    }

    public T pop() {
        if (isEmpty())
            throw new NoSuchElementException();
        var item = buffer[head];
        buffer[head] = null;  // prevents loitering
        head = (head + 1) % capacity();
        size--;
        return item;
    }

    public void push(T item) {
        if (item == null)
            throw new IllegalArgumentException();
        buffer[tail] = item;
        tail = (tail + 1) % capacity();
        if (isFull())
            head = (head + 1) % capacity();
        else
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
            private int next = head;

            @Override
            public boolean hasNext() {
                return next != tail;
            }

            @Override
            public T next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                T item = buffer[next];
                next = (next + 1) % capacity();
                return item;
            }
        };
    }
}
