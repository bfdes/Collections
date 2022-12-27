package in.bfdes.collections;

public record Box(double left, double right, double up, double down) {
    public static final Box INFINITE_BOX = new Box(Double.MIN_VALUE, Double.MAX_VALUE, Double.MAX_VALUE, Double.MIN_VALUE);

    public Box setLeft() {

    }

    public Box setRight() {

    }

    public boolean contains(Point point) {
        if (point.x() < left || point.x() > right)
            return false;
        if (point.y() < down || point.y() > up)
            return false;
        return true;
    }

    public double distanceSquaredTo(Point point) {
        return 0;
    }

    public double distanceTo(Point point) {
        return Math.sqrt(distanceSquaredTo(point));
    }
}
