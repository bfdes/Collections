package in.bfdes.collections;

/**
 * Two-dimensional point abstraction for use in the <code>KdTree</code> class.
 */
public record Point(double x, double y) {
    public static final Point ORIGIN = new Point(0, 0);
    double distanceSquaredTo(Point point) {
        if (point == null)
            throw new IllegalArgumentException();

        var dx = x - point.x;
        var dy = y - point.y;
        return Math.pow(dx, 2) + Math.pow(dy, 2);
    }

    double distanceTo(Point point) {
        return Math.sqrt(distanceSquaredTo(point));
    }

    double get(int axis) {
        return switch (axis) {
            case 0 -> x;
            case 1 -> y;
            default -> throw new IllegalArgumentException();
        };
    }
}
