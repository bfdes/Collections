using System;

namespace Collections
{
    /// <summary>
    /// Weighted directed graph edge.
    /// </summary>
    public class Edge<Vertex> : IComparable<Edge<Vertex>>
    {
        public Vertex from;
        public Vertex to;
        public double weight;

        public int CompareTo(Edge<Vertex> edge) => weight.CompareTo(edge.weight);

        public override string ToString() => $"{from} -> {to}";

        public Edge<Vertex> Complement() => new Edge<Vertex> { from = to, to = from, weight = weight };
    }
}
