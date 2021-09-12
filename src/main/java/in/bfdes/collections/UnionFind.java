package in.bfdes.collections;

public interface UnionFind<T> {
    boolean connected(T i, T j);
    void union(T i, T j);
}
