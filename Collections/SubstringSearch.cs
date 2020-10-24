using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    public static class SubstringSearch
    {
        private static readonly int r = 256;  // Search over ASCII charcters

        public static Func<string, int> Naive(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException();

            var m = pattern.Length;
            if (m == 0)
                throw new ArgumentException();

            return text =>
                {
                    var n = text.Length;

                    for (int i = 0; i <= n - m; i++)
                    {
                        int j;
                        for (j = 0; j < m; j++)
                        {
                            if (text[i + j] != pattern[j])
                                break;
                        }
                        if (j == m)
                            return i;
                    }
                    return -1;
                };
        }

        public static Func<IEnumerable<char>, int> KnuthMorrisPratt(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException();

            int m = pattern.Length;
            if (m == 0)
                throw new ArgumentException();

            int[,] dfa = new int[r, m];
            dfa[pattern[0], 0] = 1;
            for (int s = 0, j = 1; j < m; j++)
            {
                for (int i = 0; i < r; i++)
                    dfa[i, j] = dfa[i, s];
                dfa[pattern[j], j] = j + 1;
                s = dfa[pattern[j], s];
            }
            return textStream =>
                {
                    int index, state;
                    index = state = 0;
                    foreach (var c in textStream)
                    {
                        if (state == m)
                            return index - m;
                        state = dfa[c, state];
                        index++;
                    }
                    if (state == m)
                        return index - m;
                    return -1;
                };
        }

        public static Func<string, int> BoyerMoore(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException();
            // Skip table for the bad chracter heuristic:
            // skipTable[c] gives the index of the last occurance of c in the pattern
            var m = pattern.Length;
            if (m == 0)
                throw new ArgumentException();

            var skipTable = new int[r];
            for (int c = 0; c < r; c++)
                skipTable[c] = -1;
            for (int i = 0; i < m; i++)
                skipTable[pattern[i]] = i;

            return text =>
                {
                    int n = text.Length;
                    int skip;
                    for (int i = 0; i <= n - m; i += skip)
                    {
                        skip = 0;
                        for (int j = m - 1; j >= 0; j--)
                        {
                            if (pattern[j] != text[i + j])
                            {
                                // The last occurance of pattern[j] could be to the right of the mismatch
                                skip = Math.Max(1, j - skipTable[text[i + j]]);
                                break;
                            }
                        }
                        if (skip == 0)
                            return i;
                    }

                    return -1;
                };
        }

        /// <summary>
        /// Los Angelese variant of the Rabin-Karp algorithm.
        /// </summary>
        public static Func<string, int> RabinKarp(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException();

            var q = 997;
            var m = pattern.Length;
            if (m == 0)
                throw new ArgumentException();

            long Hash(string s)
            {
                long h = 0;
                foreach (var c in s.Take(m))
                    h = (h * r + c) % q;
                return h;
            }

            // Precompute r^(m-1) % q for use in removing leading digit
            long R = 1;
            for (int i = 1; i < m; i++)
                R = (r * R) % q;

            var patternHash = Hash(pattern);

            return text =>
                {
                    var n = text.Length;
                    var textHash = Hash(text);
                    if (patternHash == textHash)
                        return 0;
                    for (int i = m; i < n; i++)
                    {
                        textHash = (textHash + q - R * text[i - m] % q) % q;  // Remove contribution from leading digit
                        textHash = (textHash * r + text[i]) % q;  // Add contribution from trailing digit
                        var start = i - m + 1;
                        if (patternHash == textHash)
                            return start;
                    }

                    return -1;
                };
        }
    }
}
