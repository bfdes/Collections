using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    public class ResizingBuffer<T> : IQueue<T> where T : class
    {
        // Also present in CircularBuffer
        private T[] buffer;
        private int head = 0;  // read pointer
        private int tail = 0;  // write pointer

        private int size = 0;

        /// <summary>
        /// Initialises an empty resizing buffer.
        /// </summary>
        /// <param name="capacity">Initial capacity with default value</param>
        public ResizingBuffer(int capacity = 10)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            buffer = new T[capacity];
        }

        public ResizingBuffer(params T[] elements) : this(Math.Max(1, elements.Length))
        {
            foreach (var element in elements)
                Enqueue(element);
        }

        private void Resize(int size)
        {
            var temp = new T[size];
            for (int i = 0; i < this.size; i++)
            {
                temp[i] = buffer[(head + i) % buffer.Length];
            }
            buffer = temp;
            head = 0;
            tail = this.size;
        }

        public T Dequeue()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty buffer");
            }

            if (size == buffer.Length / 4)
            {
                Resize(buffer.Length / 2);
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
                Resize(2 * Capacity());
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

        private bool IsFull() => size == buffer.Length;  // Not relevant to client

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
