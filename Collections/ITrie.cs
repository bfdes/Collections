using System.Collections.Generic;

namespace Collections
{
    public interface ITrie<Value> : IMap<string, Value>
    {
        /// <summary>
        /// Returns all the keys with the given prefix
        /// </summary>
        IEnumerable<string> Keys(string prefix);

        /// <summary>
        /// Returns the longest prefix of the given query string.
        /// </summary>
        string LongestPrefix(string query);
    }
}
