using System;
using Kms.Core;
using Kms.Core.Utils.Extensions;
using Kms.Crypto.Models;
using Kms.Crypto.Models.DTO;
using Kms.Crypto.Models.Enum;
using static Kms.Core.CipherKey.Types;

namespace Kms.Crypto.Utils
{
    /// <summary>
    /// CipherKey Utilities
    /// </summary>
    public class CipherKeyUtils
    {
        private const int DefaultKeyExpireYear = 1;

        /// <summary>
        /// Create CipherKey object
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="base64Key">Base64 key</param>
        /// <param name="meta">KeyMetadata</param>
        /// <returns>CipherKey</returns>
        public static CipherKey Create(
            KeyTypeEnum keyType, string base64Key, KeyMetadata meta)
        {
            return InitCipherKey(keyType, meta, base64Key, string.Empty);
        }

        /// <summary>
        /// Create CipherKey object
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="publicKey">Public key</param>
        /// <param name="privateKey">Private key</param>
        /// <param name="meta">KeyMetadata</param>
        /// <returns>CipherKey</returns>
        public static CipherKey Create(
            KeyTypeEnum keyType, string publicKey, string privateKey, KeyMetadata meta)
        {
            return InitCipherKey(keyType, meta, key1: publicKey, key2: privateKey);
        }

        /// <summary>
        /// Create CipherKey object
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="meta">KeyMetadata</param>
        /// <param name="key1">Key | Public key in base64</param>
        /// <param name="key2">Key | Private key in base64</param>
        /// <returns>CipherKey</returns>
        private static CipherKey InitCipherKey(KeyTypeEnum keyType, KeyMetadata meta, string key1, string key2)
        {
            var now = DateTimeOffset.Now;
            var keyId = Guid.NewGuid().ToString();
            var activeOn = meta.ActiveOn ?? now;
            var expireOn = meta.ExpireOn ?? activeOn.AddYears(DefaultKeyExpireYear);

            var key = new CipherKey
            {
                Id = keyId,
                KeyType = keyType,
                CreateOn = now.ToProtobufTimestamp(),
                ActiveOn = activeOn.ToProtobufTimestamp(),
                ExpireOn = expireOn.ToProtobufTimestamp(),
                Key1 = key1,
                Key2 = key2,
                Purpose = meta.Purpose,
                Owner = new CipherKeyOwner
                {
                    Name = meta?.Owner?.Name,
                    Host = meta?.Owner?.Host
                }
            };

            return key;
        }
    }
}
