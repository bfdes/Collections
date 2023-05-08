package in.bfdes.collections;

public class Paths<V> {
    public final V source;
    private final Map<V, Edge<V>> incomingEdges;

    public Paths(V source, Map<V, Edge<V>> incomingEdges) {
        this.source = source;
        this.incomingEdges = incomingEdges;
    }

    public boolean contains(V destination) {
        if (destination == null)
            throw new IllegalArgumentException();
        return source.equals(destination) || incomingEdges.contains(destination);
    }

    public List<Edge<V>> get(V destination) {
        if (destination == null)
            throw new IllegalArgumentException();

        if (!contains(destination))
            throw new IllegalArgumentException();

        var path = new LinkedList<Edge<V>>();
        var vertex = destination;

        while (!vertex.equals(source)) {
            var edge = incomingEdges.get(vertex);
            path.push(edge);
            vertex = edge.from();
        }
        return path;
    }
}
