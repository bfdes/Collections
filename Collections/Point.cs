using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    /// <summary>
    /// Represents an immutable point.
    /// </summary>
    public class Point : IEquatable<Point>
    {
        private double[] coordinates;

        public Point(params double[] coordinates)
        {
            this.coordinates = coordinates;
        }

        public double this[int i]
        {
            get
            {
                return coordinates[i];
            }
        }

        public IEnumerable<double> Coordinates() => coordinates.Select(i => i);

        public int Dimension() => coordinates.Length;

        public double DistanceSquaredTo(Point p)
        {
            if (Dimension() != p.Dimension())
            {
                throw new ArgumentException();
            }
            return coordinates
                .Zip(p.coordinates, (i, j) => Math.Pow(i - j, 2))
                .Sum();
        }

        public double DistanceTo(Point p) => Math.Sqrt(DistanceSquaredTo(p));

        public override bool Equals(object obj) => this.Equals(obj as Point);

        public bool Equals(Point p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
                return false;

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
                return true;

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
                return false;

            return coordinates.SequenceEqual(p.coordinates);
        }

        public static bool operator ==(Point lhs, Point rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Point lhs, Point rhs) => !(lhs == rhs);

        public override int GetHashCode()
        {
            unchecked
            {
                return coordinates.Aggregate(17, (hash, c) => hash * 31 + c.GetHashCode());
            }
        }

        public override string ToString() => $"({string.Join(", ", coordinates)})";

        public sealed class ByDimension : Comparer<Point>
        {
            private readonly int dimension;

            public ByDimension(int dimension)
            {
                this.dimension = dimension;
            }

            public override int Compare(Point p, Point q) =>
                p[dimension].CompareTo(q[dimension]);

            public Point Min(params Point[] points) =>
                points.Aggregate((min, p) => Compare(p, min) < 0 ? p : min);
        }
    }
}
