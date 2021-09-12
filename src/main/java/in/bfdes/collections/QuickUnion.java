package in.bfdes.collections;

import java.util.HashMap;
import java.util.Map;

public class QuickUnion<T> implements UnionFind<T> {
    private final Map<T, Integer> ranks = new HashMap<>();
    private final Map<T, T> parents = new HashMap<>();

    public QuickUnion(T[] elements) {
        for (T e : elements) {
            parents.put(e, e);
            ranks.put(e, 1);
        }
    }

    private T root(T e) {
        while (!e.equals(parents.get(e))) {
            e = parents.get(e);
        }
        return e;
    }

    @Override
    public boolean connected(T i, T j) {
        if (i == null || j == null)
            throw new IllegalArgumentException();

        if (!parents.containsKey(i) || !parents.containsKey(j))
            throw new IllegalArgumentException();

        return root(i) == root(j);
    }

    @Override
    public void union(T i, T j) {
        if (i == null || j == null)
            throw new IllegalArgumentException();

        if (!parents.containsKey(i) || !parents.containsKey(j))
            throw new IllegalArgumentException();

        T rootI = root(i);
        T rootJ = root(j);
        if (rootI == rootJ) return;  // `i` and `j` are already connected
        int rankI = ranks.get(rootI);
        int rankJ = ranks.get(rootJ);

        if (rankI > rankJ) {
            ranks.put(rootI, rankI + rankJ);
            parents.put(rootI, rootJ);
        } else {
            ranks.put(rootJ, rankI + rankJ);
            parents.put(rootJ, rootI);
        }

        parents.put(rootI, rootJ);
    }
}
