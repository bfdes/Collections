package in.bfdes.collections;

public record Point(double x, double y, double z) {
    double distanceSquaredTo(Point point) {
        if (point == null)
            throw new IllegalArgumentException();
        return Math.pow(x - point.x, 2) + Math.pow(y - point.y, 2) + Math.pow(z - point.z, 2);
    }

    double distanceTo(Point point) {
        return Math.sqrt(distanceSquaredTo(point));
    }
}
