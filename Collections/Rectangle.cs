using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    /// <summary>
    /// Represents an immutable hyperrectagle. 
    /// </summary>
    public class Rectangle : IEquatable<Rectangle>
    {
        private readonly (double left, double right)[] limits;
        private static readonly EqualityComparer<ValueTuple<double, double>> comparer
            = EqualityComparer<ValueTuple<double, double>>.Default;


        public Rectangle(params (double left, double right)[] limits)
        {
            this.limits = limits;
        }

        public IEnumerable<(double left, double right)> Limits() => limits.Select(i => i);

        public int Dimension() => limits.Length;

        public static Rectangle InfiniteBox(int dimension = 3)
        {
            var limits = Enumerable
                .Range(0, dimension)
                .Select(_ => (double.MinValue, double.MaxValue))
                .ToArray();
            return new Rectangle(limits);
        }

        /// <summary>
        /// Returns a new rectangle with the left boundary of dimension j set to the axis defined by j = v.
        /// </summary>
        public Rectangle SetLeft(int j, double v)
        {
            var truncated = limits
                .Select((t, i) => i == j ? (v, t.right) : t)
                .ToArray();
            return new Rectangle(truncated);
        }

        /// <summary>
        /// Returns a new rectangle with the right boundary of dimension j set to the axis defined by j = v.
        /// </summary>
        public Rectangle SetRight(int j, double v)
        {
            var truncated = limits
                .Select((t, i) => i == j ? (t.left, v) : t)
                .ToArray();
            return new Rectangle(truncated);
        }

        public bool Contains(Point p) => limits
            .Zip(p.Coordinates(), (l, r) => (l, r))
            .All(t => t.l.left <= t.r && t.r <= t.l.right);

        public bool Contains(Rectangle that) => limits
            .Zip(that.limits, (l, r) => (l, r))
            .All(t => t.r.left >= t.l.left && t.r.right <= t.l.right);

        public bool Intersects(Rectangle that) => limits
            .Zip(that.limits, (l, r) => (l, r))
            .All(t => t.r.left <= t.l.right && t.r.right >= t.l.left);

        /// <summary>
        /// Returns the shortest distance squared between this rectangle and point p.
        /// </summary>
        public double DistanceSquaredTo(Point p)
        {
            // The shortest distance is:
            // i) the distance to the closest vertex or edge, if the point is outside the rectangle
            // ii) 0 otherwise
            double Di((double left, double right) bound, double v)
            {
                if (v < bound.left)
                {
                    return bound.left - v;
                }
                else if (v > bound.right)
                {
                    return v - bound.right;
                }
                else
                {
                    return 0;
                }
            }

            return limits
                .Zip(p.Coordinates(), Di)
                .Select(di => Math.Pow(di, 2))
                .Sum();
        }

        public double DistanceTo(Point p) => Math.Sqrt(DistanceSquaredTo(p));

        public override bool Equals(object obj) => this.Equals(obj as Rectangle);

        public bool Equals(Rectangle r)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(r, null))
                return false;

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, r))
                return true;

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != r.GetType())
                return false;

            return limits.SequenceEqual(r.limits);
        }

        public static bool operator ==(Rectangle lhs, Rectangle rhs)
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

        public static bool operator !=(Rectangle lhs, Rectangle rhs) => !(lhs == rhs);

        public override int GetHashCode()
        {
            unchecked
            {
                return limits.Aggregate(17, (hash, pair) => hash * 31 + comparer.GetHashCode(pair));
            }
        }
    }
}
