package in.bfdes.collections;

public interface List<T> extends Iterable<T> {
    int size();

    boolean isEmpty();

    T peek();

    T pop();

    void push(T item);
}
