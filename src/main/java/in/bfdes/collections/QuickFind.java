package in.bfdes.collections;

import java.util.HashMap;
import java.util.Map;

public class QuickFind<T> implements DisjointSet<T> {
    private final Map<T, T> components = new HashMap<>();

    public QuickFind(T[] elements) {
        for (var e : elements)
            components.put(e, e);
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

        var cptI = components.get(i);
        var cptJ = components.get(j);

        if (cptI == cptJ)
            return;
        for (var entry : components.entrySet()) {
            var e = entry.getKey();
            var cpt = entry.getValue();
            if (cpt == cptI)
                components.put(e, cptJ);
        }
    }
}
