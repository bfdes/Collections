package in.bfdes.collections;

public record Edge<V>(V from, V to, double weight) implements Comparable<Edge<V>> {
    public Edge<V> complement() {
        return new Edge<>(to, from, weight);
    }

    @Override
    public int compareTo(Edge<V> edge) {
        return Double.compare(weight, edge.weight);
    }

    @Override
    public String toString() {
        return String.format("%s -( %f )-> %s", from, weight, to);
    }
}
