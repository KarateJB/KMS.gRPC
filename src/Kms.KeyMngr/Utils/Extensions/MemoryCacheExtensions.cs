using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kms.KeyMngr.Utils.Extensions
{
    /// <summary>
    /// MemoryCache extensions
    /// </summary>
    public static class MemoryCacheExtensions
    {
        /// <summary>
        /// Get all CacheEnries in MemoryCache
        /// </summary>
        /// <param name="cache">IMemoryCache</param>
        /// <param name="keyRegexPattern">Key's regression pattern</param>
        /// <param name="logger">Logger(Only logs when using Key's regression pattern)</param>
        /// <returns>List of CacheEntry</returns>
        /// <example>
        /// var ces = this.memoryCache.GetAllCacheEntries();
        /// ces.ToList().ForEach(x => this.logger.LogDebug($"{x.Key}:{JsonConvert.SerializeObject(x.Value)}"));
        /// </example>
        public static IList<ICacheEntry> GetAllCacheEntries(
            this IMemoryCache cache,
            string keyRegexPattern = "",
            ILogger logger = null)
        {
            // CacheEtries
            var cacheEntries = new List<ICacheEntry>();

            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            var cacheEntriesCollection = field.GetValue(cache) as dynamic;

            if (cacheEntriesCollection != null)
            {
                foreach (var cacheItem in cacheEntriesCollection)
                {
                    // Get the "Value" from the key/value pair which contains the cache entry
                    ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);

                    if (string.IsNullOrEmpty(keyRegexPattern))
                    {
                        // Add the cache entry to the list
                        cacheEntries.Add(cacheItemValue);
                    }
                    else
                    {
                        if (cacheItemValue.Key != null && cacheItemValue.Key.GetType().Equals(typeof(string)))
                        {
                            var cacheKey = cacheItemValue.Key.ToString();
                            var cacheVal = cacheItemValue.Value;
                            Match match = Regex.Match(cacheKey, keyRegexPattern);

                            if (match.Success)
                            {
                                // Add the cache entry to the list
                                cacheEntries.Add(cacheItemValue);

                                if (logger != null)
                                {
                                    logger.LogDebug($"Saved key: {cacheKey} and value: {JsonConvert.SerializeObject(cacheVal)}");
                                }
                            }
                        }
                    }
                }
            }

            return cacheEntries;
        }
    }
}
