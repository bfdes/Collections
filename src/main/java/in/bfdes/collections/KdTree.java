package in.bfdes.collections;

import java.util.function.BiPredicate;

public class KdTree  {
    private Node root;

    private record Node(Point point, Node left, Node right) {}

    public KdTree(Point... points) {
        if(points == null || points.length == 0)
            throw new IllegalArgumentException();

        Sorting.shuffle(points);
    }

    public boolean isEmpty() {
        return root == null;
    }

    public boolean contains(Point point) {
        if(point == null)
            throw new IllegalArgumentException();

        var contains = new BiPredicate<Node, Integer>() {
            @Override
            public boolean test(Node node, int ) {
                return false;
            }
        }
    }

    public void put(Point point) {
        if(point == null)
            throw new IllegalArgumentException();
    }

    public void delete(Point point) {
        if(point == null)
            throw new IllegalArgumentException();
    }

    public Iterable<Point> rangeSearch(Rectang)
}
