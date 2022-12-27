package in.bfdes.collections;

/**
 * Rectangle abstraction for use in the <code>KdTree</code> class.
 * @param left Lowest coordinate on the x-axis
 * @param right Highest coordinate on the x-axis
 * @param up Highest coordinate on the y-axis
 * @param down Highest coordinate on the y-axis
 */
public record Rectangle(double left, double right, double up, double down) {
    public static final Rectangle INFINITE = new Rectangle(Double.MIN_VALUE, Double.MAX_VALUE, Double.MAX_VALUE, Double.MIN_VALUE);

    public Rectangle withLeft(double left) {
        return new Rectangle(left, right, up, down);
    }

    public Rectangle withRight(double right) {
        return new Rectangle(left, right, up, down);
    }

    public Rectangle withUp(double up) {
        return new Rectangle(left, right, up, down);
    }

    public Rectangle withDown(double down) {
        return new Rectangle(left, right, up, down);
    }

    public boolean contains(Point point) {
        if (point == null)
            throw new IllegalArgumentException();

        if (point.x() < left || point.x() > right)
            return false;
        if (point.y() < down || point.y() > up)
            return false;
        return true;
    }

    public boolean contains(Rectangle rectangle) {
        if (rectangle == null)
            throw new IllegalArgumentException();

        if (rectangle.left() > left || right < rectangle.right())
            return false;
        if (rectangle.up() > up || down > rectangle.down())
            return false;
        return true;
    }

    public boolean intersects(Rectangle rectangle) {
        if (rectangle == null)
            throw new IllegalArgumentException();

        if (rectangle.right() < left || right < rectangle.left())
            return false;
        if (rectangle.up() < down || up < rectangle.down())
            return false;
        return true;
    }

    public double distanceSquaredTo(Point point) {
        if (point == null)
            throw new IllegalArgumentException();

        // The shortest distance is
        // 1) the distance to the closest vertex or edge, if the point is outside the rectangle, or
        // 2) 0 otherwise.
        var dx = 0.0;
        if (point.x() < left)
            dx = left - point.x();
        else if (right < point.x())
            dx = point.x() - right;

        var dy = 0.0;
        if (point.y() < down)
            dy = down - point.y();
        else if (up < point.y())
            dy = point.y() - up;

        return Math.pow(dx, 2) + Math.pow(dy, 2);
    }

    /**
     * Returns the shortest distance between this rectangle and <code>point</code>.
     */
    public double distanceTo(Point point) {
        return Math.sqrt(distanceSquaredTo(point));
    }
}
