package in.bfdes.collections;

public record Rectangle(Tuple<Double, Double>... boundaries) {
    public int dimension() {
        return boundaries.length;
    }

    public Rectangle setLeft() {

    }

    public Rectangle setRight() {

    }

    public boolean contains(Point point) {
        for(var i = 0; i < boundaries.length; i++) {
            if(point.get < boundaries[i].first() || point)
        }
    }

    public static Rectangle infiniteBox(int dimension) {
        Tuple<Double, Double>[] boundaries = (Tuple<Double, Double>[]) new Object[dimension];
        for (var i = 0; i < dimension; i++)
            boundaries[i] = new Tuple<>(Double.MAX_VALUE, Double.MAX_VALUE);
        return new Rectangle(boundaries);
    }

    public static Rectangle infiniteBox() {
        return infiniteBox(3);
    }

    public double distanceSquaredTo(Point point) {
        return 0;
    }

    public double distanceTo(Point point) {
        return Math.sqrt(distanceSquaredTo(point));
    }
}
