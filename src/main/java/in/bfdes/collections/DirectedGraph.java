package in.bfdes.collections;

import java.util.function.Consumer;
import java.util.function.Function;
import java.util.function.Predicate;

/**
 * Adjacency list representation of a weighted, directed graph.
 */
public class DirectedGraph<V> implements Graph<V> {
    private final Map<V, Set<Edge<V>>> vertices = new HashMap<>();

    @Override
    public void add(V vertex) {
        if (!contains(vertex))
            vertices.put(vertex, new HashSet<>());
    }

    @Override
    public void add(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();
        var edges = vertices.getOrElse(edge.from(), new HashSet<>());
        edges.add(edge);
        vertices.put(edge.from(), edges);
        add(edge.to());
    }

    @Override
    public boolean contains(V vertex) {
        if (vertex == null)
            throw new IllegalArgumentException();
        return vertices.contains(vertex);
    }

    @Override
    public boolean contains(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();
        var edges = vertices.getOrElse(edge.from(), new HashSet<>());
        return edges.contains(edge);
    }

    @Override
    public void remove(V vertex) {
        if (vertex == null)
            throw new IllegalArgumentException();
        for (var edge : edges())
            if (edge.to().equals(vertex))
                remove(edge);
        vertices.delete(vertex);
    }

    @Override
    public void remove(Edge<V> edge) {
        if (edge == null)
            throw new IllegalArgumentException();
        var edges = vertices.get(edge.from());
        edges.remove(edge);
    }

    @Override
    public boolean isEmpty() {
        return vertices.isEmpty();
    }

    @Override
    public int size() {
        return vertices.size();
    }

    @Override
    public Iterable<V> vertices() {
        return vertices.keys();
    }

    @Override
    public Iterable<Edge<V>> edges() {
        var edges = new LinkedList<Edge<V>>();
        for (var vertex : vertices())
            for (var edge : edges(vertex))
                edges.push(edge);
        return edges;
    }

    /**
     * Returns the outgoing edges of a vertex.
     */
    @Override
    public Iterable<Edge<V>> edges(V vertex) {
        return vertices.get(vertex);
    }

    public DirectedGraph<V> transpose() {
        var graph = new DirectedGraph<V>();
        for (var edge : edges())
            graph.add(edge.transpose());
        return graph;
    }

    /**
     * Returns `true` if this graph is cyclic in O(V+E) time, `false` otherwise.
     */
    public boolean isCyclic() {
        // Use recursive DFS to determine whether a graph is cyclic.
        // In a depth-key trace of a cyclic graph at least one vertex will appear twice.
        var visitedVertices = new HashSet<V>();
        var stack = new HashSet<V>();

        var isCyclic = new Predicate<V>() {
            @Override
            public boolean test(V vertex) {
                if (stack.contains(vertex))
                    return true;  // `vertex` encountered twice in a depth-key trace
                if (visitedVertices.contains(vertex))
                    return false;  // `vertex` was processed in some other depth-key trace
                visitedVertices.add(vertex);
                stack.add(vertex);

                for (var edge : edges(vertex))
                    if (test(edge.to()))
                        return true;
                stack.remove(vertex);
                return false;
            }
        };

        // Vertices may not all be reachable from one another.
        // So run a DFS trace on every unvisited vertex until a cycle is found.
        for (var vertex : vertices())
            if (isCyclic.test(vertex))
                return true;
        return false;
    }

    public enum VertexOrder {
        REVERSE_POST_ORDER, ANY_ORDER
    }

    public Iterable<V> vertices(VertexOrder order) {
        if (order == VertexOrder.ANY_ORDER)
            return vertices.keys();

        // Vertices requested in reverse postorder

        var visitedVertices = new HashSet<V>();
        var stack = new LinkedList<V>();

        var collect = new Consumer<V>() {
            @Override
            public void accept(V vertex) {
                visitedVertices.add(vertex);
                for (var edge : edges(vertex))
                    if (!visitedVertices.contains(edge.to()))
                        accept(edge.to());
                stack.push(vertex);
            }
        };

        /*
        Invoke `collect` on all vertices to account for the fact that:
        1. Not all vertices are reachable from one another
        2. The key vertex we call `collect` on might have incoming links
         */
        for (var vertex : vertices.keys())
            if (!visitedVertices.contains(vertex))
                collect.accept(vertex);
        return stack;
    }

    public Iterable<DirectedGraph<V>> components() {
        // Kosaraju-Sharir algorithm
        // 1. Return vertices of G^R in reverse postorder
        // 2. Run DFS on G, visiting unmarked vertices in the order above

        // 1
        var vertices = transpose().vertices(VertexOrder.REVERSE_POST_ORDER);

        // 2
        var components = new LinkedList<DirectedGraph<V>>();
        var visitedVertices = new HashSet<V>();

        // Creates and returns a subgraph containing vertices reachable by `source` only
        var component = new Function<V, DirectedGraph<V>>() {
            @Override
            public DirectedGraph<V> apply(V source) {
                var component = new DirectedGraph<V>();
                var stack = new LinkedList<V>();
                stack.push(source);

                while (!stack.isEmpty()) {
                    var vertex = stack.pop();
                    visitedVertices.add(vertex);
                    for (var edge : edges(vertex)) {
                        var neighbour = edge.to();
                        if (visitedVertices.contains(neighbour))
                            continue;  // `neighbour` must be part of `component`
                        component.add(edge);
                        stack.push(neighbour);
                    }
                }
                return component;
            }
        };

        for (var vertex : vertices)
            if (!visitedVertices.contains(vertex))
                components.push(component.apply(vertex));
        return components;
    }

    public enum Strategy {
        BELLMAN_FORD, DIJKSTRA, TARJAN
    }

    private enum SearchState {
        UNVISITED, STARTED, FINISHED
    }

    public Iterable<V> vertices(V source) {
        var stack = new LinkedList<V>();
        var searchStates = new HashMap<V, SearchState>();
        for (var vertex : vertices())
            searchStates.put(vertex, SearchState.UNVISITED);

        var collect = new Consumer<V>() {
            @Override
            public void accept(V vertex) {
                switch (searchStates.get(vertex)) {
                    case UNVISITED -> {
                        searchStates.put(vertex, SearchState.STARTED);
                        for (var edge : edges(vertex))
                            accept(edge.to());
                        searchStates.put(vertex, SearchState.FINISHED);
                        stack.push(vertex);
                    }
                    case FINISHED -> {
                    }
                    case STARTED -> throw new IllegalArgumentException();   // Cycle detected
                }
            }
        };

        collect.accept(source);

        return stack;
    }

    public Paths<V> shortestPaths(V source, Strategy strategy) {
        if (source == null || strategy == null)
            throw new IllegalArgumentException();

        var distances = new HashMap<V, Double>();
        for (var vertex : vertices())
            distances.put(vertex, Double.MAX_VALUE);
        distances.put(source, 0d);

        var incomingEdges = new HashMap<V, Edge<V>>();

        return switch (strategy) {
            case BELLMAN_FORD -> {
                Predicate<Edge<V>> relax = edge -> {
                    var newDistance = distances.get(edge.from()) + edge.weight();
                    if (newDistance < distances.get(edge.to())) {
                        distances.put(edge.to(), newDistance);
                        incomingEdges.put(edge.to(), edge);
                        return true;
                    }
                    return false;
                };

                for (var i = 0; i < size(); i++) {
                    var relaxed = false;
                    for (var edge : edges())
                        relaxed = relaxed || relax.test(edge);
                    if (!relaxed)
                        break;
                    if (i == size() - 1)
                        throw new IllegalStateException();  // Negative cycle detected
                }
                yield new Paths<>(source, incomingEdges);
            }
            case DIJKSTRA -> {
                var adjacentVertices = new  BinaryHeap<V, Double>();
                adjacentVertices.push(source, 0d);

                Consumer<Edge<V>> relax = edge -> {
                    var newDistance = distances.get(edge.from()) + edge.weight();
                    if (newDistance >= distances.get(edge.to()))
                        return;
                    distances.put(edge.to(), newDistance);
                    adjacentVertices.push(edge.to(), newDistance);
                    incomingEdges.put(edge.to(), edge);
                };

                while (!adjacentVertices.isEmpty()) {
                    var neighbour = adjacentVertices.pop();
                    for (var edge : edges(neighbour))
                        relax.accept(edge);
                }
                yield new Paths<>(source, incomingEdges);
            }
            case TARJAN -> {
                Consumer<Edge<V>> relax = edge -> {
                    var newDistance = distances.get(edge.from()) + edge.weight();
                    if (newDistance < distances.get(edge.to())) {
                        distances.put(edge.to(), newDistance);
                        incomingEdges.put(edge.to(), edge);
                    }
                };

                for (var vertex : vertices(source))
                    for (var edge : edges(vertex))
                        relax.accept(edge);

                yield new Paths<>(source, incomingEdges);
            }
        };
    }
}
