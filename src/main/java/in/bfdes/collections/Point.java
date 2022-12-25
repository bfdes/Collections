package in.bfdes.collections;

public record Point(double x, double y, double z) {
    double distanceSquaredTo(Point point) {
        if (point == null)
            throw new IllegalArgumentException();
        var dx = x - point.x;
        var dy = y - point.y;
        var dz = z - point.z;
        return Math.pow(dx, 2) + Math.pow(dy, 2) + Math.pow(dz, 2);
    }

    double distanceTo(Point point) {
        return Math.sqrt(distanceSquaredTo(point));
    }
}
