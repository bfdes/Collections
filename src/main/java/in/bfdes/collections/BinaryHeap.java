package in.bfdes.collections;

import java.util.function.Predicate;

public class BinaryHeap<K extends Comparable<K>> implements PriorityQueue<K> {
    private K[] heap;
    private int size = 0;

    public BinaryHeap() {
        this(10);
    }

    public BinaryHeap(int initialCapacity) {
        /*
        A (minimum) heap-ordered complete binary tree is a binary tree which is
        1. completely balanced, except possibly for the lowest levels, and
        2. one where the parent node's key is at most as large as that of its children.
        In the array representation:
        - k's parent is given by k/2.
        - k's children are 2*k, 2*k+1
        - The heap invariant states heap[k] <= Min(heap[2*k], heap[2*k+1])
         */
        if (initialCapacity < 1)
            throw new IllegalArgumentException();
        heap = (K[]) new Object[initialCapacity + 1];
    }

    @Override
    public void push(K key) {
        if (isFull())
            resize(2 * heap.length);
        heap[++size] = key;
        swim(size);
    }

    @Override
    public K peek() {
        if (isEmpty())
            throw new IllegalStateException();
        return heap[1];
    }

    @Override
    public K pop() {
        if (isEmpty())
            throw new IllegalStateException();
        var smallestItem = heap[1];
        swap(1, size);
        heap[size--] = null; // prevents loitering
        sink(1);

        // Lazily reduces the size of the backing array, if necessary
        if (size == heap.length / 4)
            resize(heap.length / 2);

        return smallestItem;
    }

    @Override
    public int size() {
        return size;
    }

    @Override
    public boolean isEmpty() {
        return size == 0;
    }

    private boolean isFull() {
        return size == heap.length - 1;
    }

    /**
     * Returns `true` if this binary tree is heap ordered, `false` otherwise.
     * Note: A `BinaryHeap` should always be in heap order -- this method is for debugging.
     */
    public boolean isHeapOrdered() {
        var isHeapOrdered = new Predicate<Integer>() {
            @Override
            public boolean test(Integer k) {
                if (k > size)
                    return true;
                var l = 2 * k;
                var r = 2 * k + 1;
                if (l <= size && isGreater(k, l))
                    return false;
                if (r <= size && isGreater(k, r))
                    return false;
                return test(l) && test(r);
            }
        };
        return isHeapOrdered.test(1);
    }

    private void resize(int newCapacity) {
        var newHeap = (K[]) new Object[newCapacity];
        for (var i = 0; i < size; i++)
            newHeap[i] = heap[i];
        heap = newHeap;
    }

    private void sink(int k) {
        while (2 * k <= size) {
            var smallest = 2 * k < size && isGreater(2 * k, 2 * k + 1) ? 2 * k + 1 : 2 * k;
            if (!isGreater(k, smallest)) break;
            swap(k, smallest);
            k = smallest;
        }
    }

    private void swim(int k) {
        while (k > 1 && isGreater(k / 2, k)) {
            swap(k / 2, k);
            k = k / 2;
        }
    }

    private boolean isGreater(int i, int j) {
        return heap[i].compareTo(heap[j]) > 0;
    }

    private void swap(int i, int j) {
        var tmp = heap[i];
        heap[i] = heap[j];
        heap[j] = tmp;
    }
}
