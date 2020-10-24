using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// List implementation using inner <c>Node</c> classes.
    /// </summary>
    public class LinkedList<T> : IList<T>
    {
        private Node head;
        private int size = 0;

        private class Node
        {
            public T payload;
            public Node next;
        }

        public LinkedList(params T[] elements)
        {
            foreach (var element in elements)
                Push(element);
        }

        public T Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty list");
            }
            return head.payload;
        }

        public T Pop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty list");
            }
            var payload = head.payload;
            head = head.next;
            size--;
            return payload;
        }

        public void Push(T payload)
        {
            head = new Node
            {
                payload = payload,
                next = head
            };
            size++;
        }

        public int Size() => size;

        public bool IsEmpty() => size == 0;

        public IEnumerator<T> GetEnumerator()
        {
            var current = head;
            while (current != null)
            {
                yield return current.payload;
                current = current.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
