package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class ResizingBuffer<T> implements Iterable<T> {
    private T[] buffer;
    private int head = 0;
    private int tail = 0;
    private int size = 0;

    private class BufferIterator implements Iterator<T> {
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
            next = (next + 1) % buffer.length;
            return item;
        }
    }

    public ResizingBuffer(int initialCapacity) {
        if (initialCapacity < 1)
            throw new IllegalArgumentException();
        buffer = (T[]) new Object[initialCapacity];
    }

    public ResizingBuffer() {
        this(10);
    }

    public ResizingBuffer(T[] initialItems) {
        this(Math.max(1, initialItems.length));
        for (var item : initialItems)
            push(item);
    }

    public ResizingBuffer(Iterable<T> initialItems) {
        this();
        for (var item : initialItems)
            push(item);
    }

    public int size() {
        return size;
    }

    public boolean isEmpty() {
        return size == 0;
    }

    private boolean isFull() {
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
        head = (head + 1) % buffer.length;
        size--;
        if (size == buffer.length / 4)
            resize(buffer.length / 2);
        return item;
    }

    public void push(T item) {
        if (isFull())
            resize(2 * buffer.length);
        buffer[tail] = item;
        tail = (tail + 1) % buffer.length;
        size++;
    }

    public T dequeue() {
        return pop();
    }

    public void enqueue(T item) {
        push(item);
    }

    private void resize(int newCapacity) {
        var newBuffer = (T[]) new Object[newCapacity];
        for (int i = 0; i < size; i++) {
            newBuffer[i] = buffer[(head + i) % buffer.length];
        }
        buffer = newBuffer;
        head = 0;
        tail = size - 1;
    }

    @Override
    public Iterator<T> iterator() {
        return new BufferIterator();
    }
}
