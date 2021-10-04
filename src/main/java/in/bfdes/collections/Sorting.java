package in.bfdes.collections;

import java.util.Comparator;
import java.util.Random;

public class Sorting {
    private static final int MERGESORT_THRESHOLD = 100;

    public static <T> void shuffle(T[] array) {
        var rng = new Random();
        for (int i = 0; i < array.length; i++) {
            var r = rng.nextInt(i + 1);
            var tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }
    }

    public static <T extends Comparable<T>> boolean isSorted(T[] array) {
        return isSorted(array, Comparator.naturalOrder());
    }

    public static <T> boolean isSorted(T[] array, Comparator<T> comparator) {
        for (int i = 0; i < array.length - 1; i++) {
            var isLess = comparator.compare(array[i + 1], array[i]) < 0;
            if (isLess)
                return false;
        }
        return true;
    }

    public static <T extends Comparable<T>> void selectionSort(T[] array) {
        /*
            Iterate through the input array `a`. In iteration `i`,
            - find the index `m` of the smallest remaining entry,
            - swap `a[i]` and `a[m]`.
            Invariant: Before the `i`-th iteration, all the elements left of `i` are in sorted order.
            Selection sort requires n^2 / 2 compares and at most n exchanges.
         */
        selectionSort(array, Comparator.naturalOrder());
    }

    public static <T> void selectionSort(T[] array, Comparator<T> comparator) {
        int n = array.length;
        for (int i = 0; i < n - 1; i++) {
            int min = i;
            for (int j = i; j < n; j++) {
                var isLess = comparator.compare(array[j], array[min]) < 0;
                if (isLess)
                    min = j;
            }
            var tmp = array[i];
            array[i] = array[min];
            array[min] = tmp;
        }
    }

    public static <T extends Comparable<T>> void insertionSort(T[] array) {
        /*
            Iterate through the input array `a`. In iteration `i` swap `a[i]` with each larger entry to its left.
            Invariant: Before the `i`-th iteration, all the elements left of `i` are in ascending order.
            Insertion sort requires O(n^2) compares and exchanges.
         */
        insertionSort(array, Comparator.naturalOrder());
    }

    public static <T> void insertionSort(T[] array, Comparator<T> comparator) {
        for (int i = 0; i < array.length; i++) {
            for (int j = i; j > 0; j--) {
                var isLess = comparator.compare(array[j], array[j - 1]) < 0;
                if (isLess) {
                    var tmp = array[j];
                    array[j] = array[j - 1];
                    array[j - 1] = tmp;
                } else break;
            }
        }
    }

    public static <T extends Comparable<T>> void mergeSort(T[] array) {
        mergeSort(array, Comparator.naturalOrder());
    }

    public static <T> void mergeSort(T[] array, Comparator<T> comparator) {
        int n = array.length;
        var auxArray = (T[]) new Object[n];
        for (int size = 1; size < n; size *= 2)
            for (int i = 0; i < n - size; i += 2 * size)
                merge(array, auxArray, i, i + size - 1, Math.min(i + 2 * size - 1, n - 1), comparator);
    }

    private static <T> void merge(T[] array, T[] auxArray, int low, int mid, int high, Comparator<T> cmp) {
        for (int i = low; i < high; i++)
            auxArray[i] = array[i];

        int i = low;
        int j = mid + 1;
        for (int k = low; k < high; k++)
            if (i > mid)
                array[k] = auxArray[j++];
            else if (j > high)
                array[k] = auxArray[i++];
            else if (cmp.compare(array[i], array[j]) > 0)
                array[k] = auxArray[j++];  // stable implementation
            else
                array[k] = auxArray[i++];
    }
}
