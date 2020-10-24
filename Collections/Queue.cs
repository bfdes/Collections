using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Linked list implementation of a queue.
    /// </summary>
    public class Queue<T> : IQueue<T>
    {
        private Node head;  // refers to the first element of the queue
        private Node tail;  // refers to the last element of the queue
        private int size = 0;

        private class Node
        {
            public T payload;
            public Node next;
        }

        public Queue(params T[] elements)
        {
            foreach (var element in elements)
                Enqueue(element);
        }

        public T Dequeue()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException();
            }
            var payload = head.payload;
            head = head.next;
            size--;
            return payload;
        }

        public void Enqueue(T payload)
        {
            var last = new Node { payload = payload };
            if (IsEmpty())
            {
                head = last;
                tail = last;
            }
            else
            {
                tail.next = last;
                tail = last;
            }
            size++;
        }

        public T Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException();
            }
            return head.payload;
        }

        public void Push(T payload)
        {
            Enqueue(payload);
        }

        public T Pop() => Dequeue();

        public int Size() => size;

        public bool IsEmpty() => size == 0;

        public IEnumerator<T> GetEnumerator()
        {
            var current = head;
            while (current != tail)
            {
                yield return current.payload;
                current = current.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
