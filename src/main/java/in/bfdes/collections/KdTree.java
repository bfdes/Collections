package in.bfdes.collections;

import java.util.Comparator;
import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.BiFunction;

public class KdTree implements Iterable<Point> {
    public final static int DIMENSION = 2;

    private Node root;

    private static class Node {
        public Point point;
        public Node left;
        public Node right;

        public Node(Point point, Node left, Node right) {
            this.point = point;
            this.left = left;
            this.right = right;
        }

        public Node(Point point) {
            this(point, null, null);
        }
    }

    public KdTree(Point... points) {
        if (points == null)
            throw new IllegalArgumentException();

        Sorting.shuffle(points);

        var kdTree = new TriFunction<Integer, Integer, Integer, Node>() {
            @Override
            public Node apply(Integer low, Integer high, Integer axis) {
                if (high < low)
                    return null;

                Comparator<Point> pointComparator = Comparator.comparingDouble(p -> p.get(axis));
                Sorting.quickSort(points, low, high, pointComparator);

                var mid = low + (high - low) / 2;
                return new Node(
                        points[mid],
                        apply(low, mid - 1, next(axis)),
                        apply(mid + 1, high, next(axis))
                );
            }
        };

        root = kdTree.apply(0, points.length - 1, 0);
    }

    public boolean isEmpty() {
        return root == null;
    }

    public boolean contains(Point point) {
        if (point == null)
            throw new IllegalArgumentException();

        var node = root;
        var axis = 0;

        while (node != null) {
            if (node.point == point)
                return true;

            var diff = point.get(axis) - node.point.get(axis);
            if (diff < 0)
                node = node.left;
            else
                node = node.right;
            axis = next(axis);
        }
        return false;
    }

    public void put(Point point) {
        if (point == null)
            throw new IllegalArgumentException();
        var put = new BiFunction<Node, Integer, Node>() {
            @Override
            public Node apply(Node node, Integer axis) {
                if (node == null)
                    return new Node(point);

                var diff = point.get(axis) - node.point.get(axis);
                if (diff < 0)
                    return apply(node.left, next(axis));
                return apply(node.right, next(axis));
            }
        };

        root = put.apply(root, 0);
    }

    public void delete(Point point) {
        if (point == null)
            throw new IllegalArgumentException();

        var min = new TriFunction<Node, Integer, Integer, Point>() {
            @Override
            public Point apply(Node node, Integer currentAxis, Integer targetAxis) {
                if (node == null) return null;

                if (targetAxis == currentAxis) {
                    if (node.left == null)
                        return node.point;
                    return apply(node.left, next(currentAxis), targetAxis);
                }

                var leftMin = apply(node.left, next(currentAxis), targetAxis);
                var rightMin = apply(node.right, next(currentAxis), targetAxis);
                var min = node.point;

                if (leftMin != null) {
                    var diff = leftMin.get(targetAxis) - min.get(targetAxis);
                    min = diff < 0 ? leftMin : min;
                }
                if (rightMin != null) {
                    var diff = rightMin.get(targetAxis) - min.get(targetAxis);
                    min = diff < 0 ? rightMin : min;
                }
                return min;
            }
        };

        var delete = new TriFunction<Point, Node, Integer, Node>() {
            @Override
            public Node apply(Point point, Node node, Integer axis) {
                if (node == null)
                    return null;

                if (point == node.point) {
                    if (node.right != null) {
                        node.point = min.apply(node.right, next(axis), axis);
                        node.right = apply(node.point, node.right, next(axis));
                    } else if (node.left != null) {
                        node.point = min.apply(node.left, next(axis), axis);
                        node.right = apply(node.point, node.left, next(axis));
                        node.left = null;
                    } else node = null;
                } else if (point.get(axis) < node.point.get(axis))
                    node.left = apply(point, node.left, next(axis));
                else
                    node.right = apply(point, node.right, next(axis));
                return node;
            }
        };

        root = delete.apply(point, root, 0);
    }

    /**
     * Returns all the points of the tree within the given query rectangle, <code>rectangle</code>.
     */
    public Iterable<Point> rangeSearch(Rectangle rectangle) {
        if (rectangle == null)
            throw new IllegalArgumentException();

        var points = new LinkedList<Point>();

        var collect = new TriConsumer<Node, Rectangle, Integer>() {
            @Override
            public void accept(Node node, Rectangle boundingBox, Integer axis) {
                // Terminate the search if the subtree rooted at this node
                // 1) is empty, or
                // 2) does not have a bounding box that intersects the query rectangle.
                if (node == null || !boundingBox.intersects(rectangle))
                    return;
                if (rectangle.contains(node.point))
                    points.push(node.point);

                var leftBoundingBox = switch (axis) {
                    case 0 -> boundingBox.withRight(node.point.x());
                    case 1 -> boundingBox.withDown(node.point.y());
                };
                var rightBoundingBox = switch (axis) {
                    case 0 -> boundingBox.withLeft(node.point.x());
                    case 1 -> boundingBox.withUp(node.point.y());
                };

                accept(node.left, leftBoundingBox, next(axis));
                accept(node.right, rightBoundingBox, next(axis));
            }
        };

        collect.accept(root, Rectangle.INFINITE, 0);

        return points;
    }

    public Point nearestNeighbour(Point point) {
        // Implementation taken from CMU Bioinformatics lecture slides (offered by Carl Kingsford)
        if (point == null)
            throw new IllegalArgumentException();
        if (isEmpty())
            throw new IllegalStateException();

        var collect = new TriConsumer<Node, Rectangle, Integer>() {
            public Point closest; // Workaround for Java shortcoming that prevents inner classes from closing over local variables
            private double min = Double.MAX_VALUE;

            @Override
            public void accept(Node node, Rectangle boundingBox, Integer axis) {
                // Inspect a subtree if its bounding box is able to contain a point closer to that already found
                if (node == null || boundingBox.distanceSquaredTo(point) >= min)
                    return;

                var dist = point.distanceSquaredTo(node.point);  // d^2 is monotonic on d and cheaper to compute than d
                if (dist < min) {
                    min = dist;
                    closest = node.point;
                }

                var leftBoundingBox = switch (axis) {
                    case 0 -> boundingBox.withRight(node.point.x());
                    case 1 -> boundingBox.withDown(node.point.y());
                };
                var rightBoundingBox = switch (axis) {
                    case 0 -> boundingBox.withLeft(node.point.x());
                    case 1 -> boundingBox.withUp(node.point.y());
                };

                // Inspect children in the order that is likely to result in rapid pruning of the search path
                if (point.get(axis) < node.point.get(axis)) {
                    accept(node.left, leftBoundingBox, next(axis));
                    accept(node.right, rightBoundingBox, next(axis));
                } else {
                    accept(node.right, rightBoundingBox, next(axis));
                    accept(node.left, leftBoundingBox, next(axis));
                }
            }
        };

        collect.accept(root, Rectangle.INFINITE, 0);

        return collect.closest;
    }

    @Override
    public Iterator<Point> iterator() {
        return new Iterator<>() {
            private final List<Node> stack = new LinkedList<>();

            private Node node = root;

            @Override
            public boolean hasNext() {
                return node != null || !stack.isEmpty();
            }

            @Override
            public Point next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                while (node != null) {
                    stack.push(node);
                    node = node.left;
                }
                node = stack.pop();
                var point = node.point;
                node = node.right;
                return point;
            }
        };
    }

    private static int next(int axis) {
        return (axis + 1) % DIMENSION;
    }
}
