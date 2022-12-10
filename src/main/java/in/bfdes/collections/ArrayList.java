package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class ArrayList<T> implements List<T> {
    private T[] stack;
    private int head = -1;

    public ArrayList(int initialCapacity) {
        if (initialCapacity < 1)
            throw new IllegalArgumentException();
        stack = (T[]) new Object[initialCapacity];
    }

    public ArrayList() {
        this(10);
    }

    public ArrayList(T[] initialItems) {
        this(Math.max(1, initialItems.length));
        for (var item : initialItems)
            push(item);
    }

    public ArrayList(Iterable<T> initialItems) {
        for (var item : initialItems)
            push(item);
    }

    @Override
    public int size() {
        return head + 1;
    }

    @Override
    public boolean isEmpty() {
        return head == -1;
    }

    private boolean isFull() {
        return head == stack.length - 1;
    }

    @Override
    public T peek() {
        if (isEmpty())
            throw new NoSuchElementException();
        return stack[head];
    }

    @Override
    public T pop() {
        if (isEmpty())
            throw new NoSuchElementException();
        var item = stack[head];
        stack[head--] = null;
        if (head == stack.length / 4)
            resize(stack.length / 2);
        return item;
    }

    @Override
    public void push(T item) {
        if (item == null)
            throw new IllegalArgumentException();
        if (isFull())
            resize(2 * stack.length);
        stack[++head] = item;
    }

    private void resize(int newCapacity) {
        var newStack = (T[]) new Object[newCapacity];
        for (int i = 0; i <= head; i++)
            newStack[i] = stack[i];
        stack = newStack;
    }

    @Override
    public Iterator<T> iterator() {
        return new Iterator<>() {
            private int next = 0;

            @Override
            public boolean hasNext() {
                return next <= head;
            }

            @Override
            public T next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return stack[next++];
            }
        };
    }
}
