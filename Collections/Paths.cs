using System;
using System.Collections.Generic;

namespace Collections
{
    public class Paths<Vertex>
    {
        public Vertex Source { get; private set; }
        private HashMap<Vertex, Edge<Vertex>> edgeTo;

        public Paths(Vertex source, HashMap<Vertex, Edge<Vertex>> edgeTo)
        {
            Source = source;
            this.edgeTo = edgeTo;
        }

        /// <summary>
        /// Returns true if there is a path from the source vertex to <code>v</code>, false otherwise.
        /// </summary>
        public bool Contains(Vertex vertex) => vertex.Equals(Source) || edgeTo.Contains(vertex);

        /// <summary>
        /// Returns the path to <code>v</code> from <code>Source</code> (if it exists) in O(V) time.
        /// </summary>
        /// <returns>
        /// Path as a sequence of edges to follow.
        /// </returns>
        public IEnumerable<Edge<Vertex>> Get(Vertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException();

            if (!Contains(vertex))
                throw new InvalidOperationException();

            var stack = new LinkedList<Edge<Vertex>>();
            var current = vertex;

            while (!current.Equals(Source))
            {
                var e = edgeTo[current];
                stack.Push(e);
                current = e.from;
            }
            return stack;
        }
    }
}
