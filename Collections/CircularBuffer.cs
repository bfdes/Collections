using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// Fixed capacity queue of type T implemented as a cicular buffer.
    /// </summary>
    public class CircularBuffer<T> : IQueue<T> where T : class
    {
        private T[] buffer;
        private int head = 0;  // read pointer
        private int tail = 0;  // write pointer
        private int size = 0;

        public CircularBuffer(int capacity = 10)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            buffer = new T[capacity];
        }

        public CircularBuffer(params T[] elements) : this(Math.Max(1, elements.Length))
        {
            foreach (var element in elements)
                Enqueue(element);
        }

        public T Dequeue()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty buffer");
            }
            size--;
            var returnValue = buffer[head];
            buffer[head] = null;  // prevent loitering
            head = (head + 1) % Capacity();
            return returnValue;
        }

        public void Enqueue(T payload)
        {
            if (IsFull())
            {
                throw new InvalidOperationException("Buffer is full");
            }
            size++;
            buffer[tail] = payload;
            tail = (tail + 1) % Capacity();
        }

        public T Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty buffer");
            }
            return buffer[head];
        }

        public void Push(T payload)
        {
            Enqueue(payload);
        }

        public T Pop() => Dequeue();

        public int Size() => size;

        public bool IsEmpty() => size == 0;

        public bool IsFull() => size == buffer.Length;

        public int Capacity() => buffer.Length;

        public IEnumerator<T> GetEnumerator()
        {
            var current = head;
            while (current != tail)
            {
                yield return buffer[current];
                current = (current + 1) % Capacity();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
