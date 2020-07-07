using System;
using Kms.Core;

namespace Kms.KeyMngr.Factory
{
    /// <summary>
    /// Cache key factory
    /// </summary>
    public static class CacheKeyFactory
    {
        /// <summary>
        /// Prefix string for Cipher(key)'s cache key
        /// </summary>
        public const string KeyPrefixCipher = "Cipher";

        /// <summary>
        /// Prefix string for RequestCache's cache key
        /// </summary>
        public const string KeyPrefixRequestCache = "RequestCache";

        /// <summary>
        /// Key for Secret key
        /// </summary>
        /// <return>Cache key</return>
        public static string GetKeyCipher(KeyTypeEnum keyType) => $"{KeyPrefixCipher}-{keyType.ToString()}";

        /// <summary>
        /// Key for temp request's payload (before encrypted)
        /// </summary>
        /// <return>Cache key</return>
        public static string GetKeyRequestCache() => $"{KeyPrefixRequestCache}-{Guid.NewGuid()}";
    }
}
