using System;

namespace Collections
{
    internal class Double : IEquatable<Double>, IComparable<Double>
    {
        public readonly double Value;

        private Double() { }

        public Double(double value) { Value = value; }

        public static readonly Double MaxValue = new Double(double.MaxValue);

        public static readonly Double MinValue = new Double(double.MinValue);

        public int CompareTo(Double other) => Value.CompareTo(other.Value);

        public override bool Equals(object obj) => this.Equals(obj as Double);

        public bool Equals(Double other)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(other, null))
                return false;

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
                return true;

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
                return false;

            return Value == other.Value;
        }

        public static bool operator ==(Double lhs, Double rhs)
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

        public static bool operator !=(Double lhs, Double rhs) => !(lhs == rhs);

        public static Double operator +(Double lhs, Double rhs) => new Double(lhs.Value + rhs.Value);

        public static Double operator -(Double lhs, Double rhs) => new Double(lhs.Value - rhs.Value);

        public static Double operator *(Double lhs, Double rhs) => new Double(lhs.Value * rhs.Value);

        public static Double operator /(Double lhs, Double rhs) => new Double(lhs.Value / rhs.Value);

        public static bool operator <(Double lhs, Double rhs) => lhs.Value < rhs.Value;

        public static bool operator >(Double lhs, Double rhs) => lhs.Value > rhs.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }
}
