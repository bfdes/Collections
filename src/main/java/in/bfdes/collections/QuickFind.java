package in.bfdes.collections;

import java.util.HashMap;
import java.util.Map;

public class QuickFind<T> implements UnionFind<T> {
    private final Map<T, T> components = new HashMap<>();

    public QuickFind(T[] elements) {
        for(T e : elements) {
            components.put(e, e);
        }
    }

    @Override
    public boolean connected(T i, T j) {
        if (i == null || j == null)
            throw new IllegalArgumentException();

        if (!components.containsKey(i) || !components.containsKey(j))
            throw new IllegalArgumentException();

        return components.get(i) == components.get(j);
    }

    @Override
    public void union(T i, T j) {
        if (i == null || j == null)
            throw new IllegalArgumentException();

        if (!components.containsKey(i) || !components.containsKey(j))
            throw new IllegalArgumentException();

        T cptI = components.get(i);
        T cptJ = components.get(j);

        if(cptI == cptJ) {
            return; // `i` and `j` are already connected
        }
        for (Map.Entry<T, T> entry : components.entrySet()) {
            T e = entry.getKey();
            T cpt = entry.getValue();
            if(cpt == cptI) {
                components.put(e, cptJ);
            }
        }
    }
}
