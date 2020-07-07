using System;
using System.Collections.Generic;
using System.Linq;

namespace Kms.Core.Utils.Extensions
{
    /// <summary>
    /// IEnumerable extensions
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// IEnumerable Foreach extension
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="collections">Object collection</param>
        /// <param name="action">Action</param>
        public static void ForEach<T>(this IEnumerable<T> collections, Action<T> action)
        {
            if (collections == null || action == null)
            {
                throw new ArgumentNullException("collections/action");
            }

            foreach (var item in collections)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convert to aggregated string with separate string
        /// </summary>
        /// <param name="strs">String collection</param>
        /// <param name="separateStr">The separate string</param>
        /// <param name="isArrayFormat">Is formatted as array</param>
        /// <returns>Aggregated string</returns>
        public static string ToAggregatedString(this IEnumerable<string> strs, string separateStr = ",", bool isArrayFormat = true)
        {
            string rslt = string.Empty;
            if (strs != null && strs.Count() > 0)
            {
                rslt = strs.Aggregate((a, b) => $"{a}{separateStr} {b}");

                if (rslt.StartsWith(separateStr))
                {
                    rslt = rslt.Remove(0, separateStr.Length);
                }

                if (isArrayFormat)
                {
                    rslt = $"[{rslt}]";
                }
            }

            return rslt;
        }
    }
}
