using System;
using System.Collections.Generic;
using System.Linq;

namespace Moen.U.Api.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static IEnumerable<KeyValuePair<T, S>> ToKeyValuePairList<T, S>(this Dictionary<T, S> dictionary)
        {
            var list = new List<KeyValuePair<T, S>>();
            foreach (var pair in dictionary)
                list.Add(pair);
            return list;
        }

        internal static string ToQueryString(this Dictionary<string, object> dictionary)
        {
            string querystring = string.Empty;
            List<string> parameters = new List<string>();

            if (dictionary == null)
                return querystring;

            foreach (var pair in dictionary.Where(w => w.Value != null))
                parameters.Add(string.Join("=", pair.Key, Uri.EscapeDataString(pair.Value.ToString())));

            if (parameters.Count > 0)
                querystring = "?" + string.Join("&", parameters);

            return querystring;
        }
    }
}