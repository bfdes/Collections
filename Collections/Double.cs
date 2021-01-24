using System;

namespace Collections
{
    internal record Double(double Value) : IComparable<Double>
    {
        public int CompareTo(Double other) => Value.CompareTo(other.Value);

        public override string ToString() => Value.ToString();

        public static Double operator +(Double lhs, Double rhs) => new Double(lhs.Value + rhs.Value);

        public static Double operator -(Double lhs, Double rhs) => new Double(lhs.Value - rhs.Value);

        public static Double operator *(Double lhs, Double rhs) => new Double(lhs.Value * rhs.Value);

        public static Double operator /(Double lhs, Double rhs) => new Double(lhs.Value / rhs.Value);

        public static bool operator <(Double lhs, Double rhs) => lhs.Value < rhs.Value;

        public static bool operator >(Double lhs, Double rhs) => lhs.Value > rhs.Value;

        public static readonly Double MaxValue = new Double(double.MaxValue);

        public static readonly Double MinValue = new Double(double.MinValue);
    }
}
