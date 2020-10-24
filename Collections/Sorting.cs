using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Collections
{
    public static class Sorting
    {
        /// <summary>
        /// Sorts integer arrays using linear time and space.
        /// </summary>
        public static void RadixSort(int[] array)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (array.Length == 0)
                return;

            var min = array.Min();
            var max = array.Max();
            var r = max - min + 1;
            var count = new int[r + 1];
            var aux = new int[array.Length];

            foreach (var a in array)
                count[a - min + 1]++;

            for (int i = 0; i < r; i++)
                count[i + 1] += count[i];

            foreach (var a in array)
                aux[count[a - min]++] = a;

            array = aux;
        }

        public static void QuickSort(string[] array)
        {
            int CharAt(string s, int i) =>
                i < s.Length ? s[i] : -1;  // Property to ensure shorter strings preceede longer ones

            void Sort(int l, int h, int d)
            {
                if (h <= l)
                    return;
                int lt = l, gt = h;
                int v = CharAt(array[l], d);
                int i = l + 1;

                while (i <= gt)
                {
                    int t = CharAt(array[i], d);
                    if (t < v)
                        Swap(array, lt++, i++);
                    else if (t > v)
                        Swap(array, i, gt--);
                    else
                        i++;
                }

                Sort(l, lt - 1, d);
                if (v >= 0)
                    Sort(lt, gt, d + 1);
                Sort(gt + 1, h, d);
            }

            Shuffle(array);
            Sort(0, array.Length - 1, 0);
        }

        /// <summary>
        /// Sorts an array of ASCII strings of common length using linear time and space. 
        /// </summary>
        public static void LSDSort(string[] array)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (array.Length == 0)
                return;

            var l = array.First().Length;

            if (array.Any(s => s.Length != l))
                throw new ArgumentException();

            var n = array.Length;
            var aux = new string[n];  // OK to have a common auxilliary array
            var r = 256;

            // Sort all the strings according to their chars in the i-th column
            void Sort(int i)
            {
                var count = new int[r + 1];  // Cannot have common count array

                foreach (var s in array)
                    count[s[i] + 1]++;
                for (int j = 0; j < r; j++)
                    count[j + 1] += count[j];
                foreach (var s in array)
                    aux[count[s[i]]++] = s;
                array = aux;
            }

            for (int i = l - 1; i >= 0; i--)
                Sort(i);  // Sort the array of strings by their char in the least-significant position first

            // Costs: l * n time and r + n space
        }

        public static void MSDSort(string[] array)
        {
            var r = 256;
            var aux = new string[array.Length];

            int CharAt(string s, int i) =>
                i < s.Length ? s[i] : -1;  // Property to ensure shorter strings preceede longer ones

            // Sorts the subarray [l, h], starting the inspection on the d-th column of chars
            void Sort(int l, int h, int d)
            {
                if (l >= h)
                    return;
                var count = new int[r + 2];
                for (int i = l; i <= h; i++)
                    count[CharAt(array[i], d) + 2]++;  // Shift index by 2 to accomodate fix for short strings
                for (int j = 0; j < r + 1; j++)
                    count[j + 1] += count[j];
                for (int i = l; i <= h; i++)
                    aux[count[CharAt(array[i], d) + 1]++] = array[i];
                for (int i = l; i <= h; i++)
                    array[i] = aux[i - l];

                for (int j = 0; j < r; j++)
                    Sort(l + count[j], l + count[j + 1] - 1, d + 1);
            }

            Sort(0, array.Length - 1, 0);
        }


        /// <summary>
        /// Sorts an array in place in worst case quadratic time using the given comparer.
        /// </summary>
        public static void InsertionSort<T>(this T[] array, Comparer<T> comparer)
        {
            InsertionSort(array, 0, array.Length - 1, comparer);
        }

        private static void InsertionSort<T>(this T[] array, int low, int high, Comparer<T> comparer)
        {
            for (int i = low + 1; i < high + 1; i++)
            {
                for (int j = i; j > low; j--)
                {
                    if (Less(array[j], array[j - 1], comparer))
                    {
                        Swap(array, j, j - 1);  // Stable sorting algorithm
                    }
                    else
                    {
                        break;  // O(N^2) compares and exchanges; Theta(N) compares and 0 exchanges.
                    }
                }
            }
        }

        /// <summary>
        /// Sorts an array in place in worst case quadratic time.
        /// </summary>
        public static void InsertionSort<T>(this T[] array) where T : IComparable<T>
        {
            InsertionSort(array, Comparer<T>.Default);  // Comparer<T> uses the CompareTo method of T to provide a default Comparer implementation
        }

        /// <summary>
        /// Shuffles an array in place using Knuth shuffle.
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            var rnd = new Random();

            for (int i = 1; i < array.Length; i++)
            {
                var r = rnd.Next(i + 1);
                Swap(array, i, r);
            }
        }

        /// <summary>
        /// Sorts an array in linearithmic time using the given comparer.
        /// </summary>
        public static void MergeSort<T>(this T[] array, Comparer<T> comparer)
        {
            // Bottom-up implementation (only uses one stack frame), transcribed to C# from Algorithms I
            int n = array.Length;

            T[] aux = new T[n];  // Maintain a single auxilliary array for all merge operations

            // Merges the sorted subsequences [low, mid] and (mid, high]
            void Merge(int low, int mid, int high)
            {
                Debug.Assert(IsSorted(array, low, mid, comparer));
                Debug.Assert(IsSorted(array, mid + 1, high, comparer));

                for (int k = low; k <= high; k++)
                {
                    aux[k] = array[k];
                }

                int i = low, j = mid + 1;
                for (int k = low; k <= high; k++)
                {
                    if (i > mid)
                    {
                        array[k] = aux[j++];
                    }
                    else if (j > high)
                    {
                        array[k] = aux[i++];
                    }
                    else if (Less(aux[j], aux[i], comparer))
                    {
                        array[k] = aux[j++];  // n.b. Stable implementation as we take from the LHS subarray if Compare(aux[i], aux[j]) is 0
                    }
                    else
                    {
                        array[k] = aux[i++];
                    }
                }

                Debug.Assert(IsSorted(array, low, high, comparer));
            }

            for (int interval = 1; interval < n; interval = 2 * interval)
            {
                for (int i = 0; i < n - interval; i += 2 * interval)
                {
                    Merge(i, i + interval - 1, Math.Min(i + 2 * interval - 1, n - 1));
                }
            }
        }
        /// <summary>
        /// Sorts an array in linearithmic time.
        /// </summary>
        public static void MergeSort<T>(this T[] array) where T : IComparable<T> => MergeSort(array, Comparer<T>.Default);

        /// <summary>
        /// Sorts an array in place in average linearithmic time using the given comparer.
        /// </summary>
        public static void QuickSort<T>(this T[] array, Comparer<T> comparer)
        {
            // Transcribed to C# from Algorithms I
            // Involves two optimisations:
            // - three-way partitioning for improved performance when the array is saturated with duplicate keys
            // - delgates to Insertion sort when sorting smaller parititions to avoid the overhead of allocating stack frames
            Shuffle(array);  // Make worst case performance a very rare occurance
            QuickSort(array, 0, array.Length - 1, comparer);
        }

        internal static void QuickSort<T>(T[] array, int low, int high, Comparer<T> comparer)
        {
            // Partitions the array [l, h] into the segements:
            // - array[i] < pivot for all [l, lt)
            // - array[i] = pivot for all [lt, gt]
            // - array[i] > pivot for all (gt, h] where pivot = array[l] (initial value)
            (int lt, int gt) Partition(int l, int h)
            {
                int lt = l;
                int i = l;
                int gt = h;
                T pivot = array[l];

                while (i <= gt)
                {
                    int diff = comparer.Compare(array[i], pivot);
                    if (diff < 0)
                    {
                        Swap(array, i++, lt++);
                    }
                    else if (diff > 0)
                    {
                        Swap(array, i, gt--);
                    }
                    else
                    {
                        i++;
                    }
                }
                return (lt, gt);
            }

            if (high - low < 10)
            {
                InsertionSort(array, low, high, comparer);
                return;
            }

            var p = Partition(low, high);
            QuickSort(array, low, p.lt - 1, comparer);
            QuickSort(array, p.gt + 1, high, comparer);
        }

        /// <summary>
        /// Sorts an array in place in average linearithmic time.
        /// </summary>
        public static void QuickSort<T>(this T[] array) where T : IComparable<T> => QuickSort(array, Comparer<T>.Default);

        /// <summary>
        /// Sorts an array in place in linearithmic time using the given comparer.
        /// </summary>
        public static void HeapSort<T>(this T[] array, Comparer<T> comparer)
        {
            // n.b. HeapSort is both in place and fast but makes poor use of cache memory.
            // It is also not stable.
            var N = array.Length;

            // Heapify -- Recursive Theta(N) procedure
            // Here we turn the whole array into a max heap by heapifying 
            // all the subtrees rooted at [N/2, ..., 1] in reverse order.
            //  - For i > N/2 we have single element nodes. 
            //  - Moving in reverse order means the children of k are heap ordered,
            //    so a simple Sink operation on k maintains the heap invariant.
            for (int k = N / 2; k > 0; k--)
            {
                Sink(k);
            }

            // Gradually build a sorted array by moving the largest key witnessed to the end.
            while (N > 1)
            {
                Swap(1, N--);
                Sink(1); // The heap is [1, ..., N-1] and the array is [N, ...]
            }

            void Sink(int k)
            {
                while (2 * k <= N)
                {
                    var largest = 2 * k < N && Less(2 * k, 2 * k + 1) ? 2 * k + 1 : 2 * k;
                    if (!Less(k, largest)) break;
                    Swap(k, largest);
                    k = largest;
                }
            }

            // Shadow the util functions defined below as we need to account for zero-indexing

            bool Less(int i, int j) => comparer.Compare(array[i - 1], array[j - 1]) < 0;

            void Swap(int i, int j)
            {
                var temp = array[i - 1];
                array[i - 1] = array[j - 1];
                array[j - 1] = temp;
            }
        }

        /// <summary>
        /// Sorts an array in place in linearithmic time.
        /// </summary>
        public static void HeapSort<T>(this T[] array) where T : IComparable<T> => HeapSort(array, Comparer<T>.Default);

        // Sedgewick recommends using utility functions

        /// <summary>
        /// Returns true if v is less than w, false otherwise.
        /// </summary>
        private static bool Less<T>(T v, T w, Comparer<T> comparer) => comparer.Compare(v, w) < 0;

        private static void Swap<T>(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        /// <summary>
        /// Returns true if the array is sorted; false otherwise.
        /// </summary>
        public static bool IsSorted<T>(this T[] array, Comparer<T> comparer) where T : IComparable<T> => IsSorted(array, 0, array.Length - 1, comparer);

        /// <summary>
        /// Returns true if the array is sorted; false otherwise.
        /// </summary>
        public static bool IsSorted<T>(this T[] array) where T : IComparable<T> => IsSorted(array, Comparer<T>.Default);

        /// <summary>
        /// Returns true if the subsequence [low, high] is sorted; false otherwise.
        /// </summary>
        private static bool IsSorted<T>(T[] array, int low, int high, Comparer<T> comparer)
        {
            for (int i = low; i < high; i++)
            {
                if (Less(array[i + 1], array[i], comparer))
                {
                    return false;  // O(high-low) compares
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the subsequence [low, high] is sorted; false otherwise.
        /// </summary>
        private static bool IsSorted<T>(T[] array, int low, int high) where T : IComparable<T> => IsSorted(array, low, high, Comparer<T>.Default);
    }
}
