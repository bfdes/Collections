using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    /// <summary>
    /// List implementation using a dynamically-resizing array.
    /// </summary>
    public class ArrayList<T> : IList<T> where T : class
    {
        private T[] stack;
        private int head;

        public ArrayList(int capacity = 10)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            stack = new T[capacity];
            head = -1;  // initially empty
        }

        public ArrayList(params T[] elements) : this(Math.Max(1, elements.Length))
        {
            foreach (var element in elements)
                Push(element);
        }

        public T this[int i]
        {
            get
            {
                if (i > Size())
                {
                    throw new ArgumentOutOfRangeException();
                }
                return stack[i];
            }

            set
            {
                if (i > Size())
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                stack[i] = value;
            }
        }

        public bool IsEmpty() => head == -1;

        private void Resize(int size)
        {
            var temp = new T[size];
            for (int i = 0; i <= head; i++)
            {
                temp[i] = stack[i];
            }
            stack = temp;
        }

        public T Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty list");
            }
            return stack[head];
        }

        public T Pop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Empty list");
            }

            // We actively de-reference to prevent loitering,
            // which is why the implementation can only support reference types.
            var returnValue = stack[head];
            stack[head--] = null;  // cannot be confused for a null payload as we always track the head.

            // Lazily reduce the size of the backing array if necessary
            if (head == stack.Length / 4)
            {
                Resize(stack.Length / 2);
            }

            return returnValue;
        }

        public void Push(T payload)
        {
            // Lazily increase the size of the backing array if necessary
            if (head == stack.Length - 1)
            {
                Resize(2 * stack.Length);
            }
            stack[++head] = payload;
        }

        public int Size() => head + 1;

        public int Capacity() => stack.Length;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = head; i > -1; i--)
            {
                yield return stack[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
