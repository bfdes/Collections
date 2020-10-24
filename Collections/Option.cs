using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    using static Option;

    public static class Option
    {
        public static Option<U> Some<U>(U value) => new Option<U>(value);

        public static Option<U> None<U>() => new Option<U>();
    }

    interface IOption<out T> : IEnumerable<T> { }

    public sealed class Option<T> : IOption<T>, IEquatable<Option<T>>
    {
        public readonly T Value;
        private readonly bool hasValue;

        internal Option()
        {
            hasValue = false;
        }

        internal Option(T value)
        {
            hasValue = true;
            Value = value;
        }

        public bool Contains(T value) => hasValue && Value.Equals(value);

        public bool IsEmpty() => !hasValue;

        public T GetOrElse(T defaultValue) => hasValue ? Value : defaultValue;

        public Option<U> FlatMap<U>(Func<T, Option<U>> f) => hasValue ? f(Value) : None<U>();

        public Option<U> Map<U>(Func<T, U> f) => FlatMap(t => Some(f(t)));

        public Option<T> Filter(Predicate<T> f) => FlatMap(t => f(t) ? Some(t) : None<T>());

        public IEnumerator<T> GetEnumerator()
        {
            // An option can be thought of as a container that can hold one object
            if (hasValue)
            {
                yield return Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object obj) => this.Equals(obj as Option<T>);

        public bool Equals(Option<T> other)
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

            // One option is equal to another if they are both empty or if their values are equal
            if (!hasValue && !other.hasValue)
                return true;
            if (hasValue && other.hasValue)
                return Value.Equals(other.Value);
            return false;
        }

        public override int GetHashCode()
        {
            // n.b. The hahscode of empty options and options with value null is the same
            unchecked
            {
                return Value.GetHashCode();
            }
        }
    }
}
