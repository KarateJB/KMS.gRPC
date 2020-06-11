using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Core.Models.Config;
using Kms.Core.Models.Config.Server;
using Kms.Core.Utils;
using Kms.Core.Utils.Extensions;
using Kms.Crypto.Factory;
using Kms.Crypto.Models.DTO;
using Kms.Crypto.Services;
using Kms.gRPC.Services.Cache;
using Kms.gRPC.Utils.Factory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.DataProtection
{
    /// <summary>
    /// Key Vault
    /// </summary>
    public class KeyVault : IKeyVault
    {
        private const int DefaultKeyExpireYear = 1;
        private readonly AppSettings appSettings = null;
        private readonly ILogger<KeyVault> logger = null;
        private readonly RedisService redis = null;

        public KeyVault(
            IOptions<AppSettings> configuration,
            ILogger<KeyVault> logger,
            RedisService redis)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;
            this.redis = redis;
        }

        /// <summary>
        /// Create a new TripleDES key
        /// </summary>
        /// <param name="keyMeta">Key metadata</param>
        /// <returns>New key</returns>
        public async Task<CipherKey> CreateTripleDesAsync(KeyMetadata keyMeta)
        {
            if (keyMeta == null || keyMeta?.Owner == null)
            {
                throw new ArgumentNullException($"{nameof(KeyMetadata)} or Owner");
            }

            #region Get base64 Key
            var base64Key = string.Empty;
            var secret = SecretFactory.Create();
            using (var tds = new TripleDesService())
            {
                base64Key = await tds.CreateKeyAsync(secret);

                if (string.IsNullOrEmpty(base64Key))
                {
                    throw new Exception($"Create TripleDES key fails on CreateTripleDesAsync!");
                }
            }
            #endregion

            #region Create key object
            keyMeta.ExpireOn = this.GetExpireOn(this.appSettings?.Kms?.KeyVault?.Symmetric?.DefaultKeyExpire);
            CipherKey key = await this.GenCipherKeyAsync(KeyTypeEnum.TripleDes, keyMeta, base64Key);
            #endregion

            #region Save key to KeyVault
            await this.SaveKeyAsync(key.Id, key);
            #endregion

            return await Task.FromResult(key);
        }

        /// <summary>
        /// Create a new Secret key
        /// </summary>
        /// <param name="keyMeta">Key metadata</param>
        /// <returns>New key</returns>
        public async Task<CipherKey> CreateSharedSecretAsync(KeyMetadata keyMeta)
        {
            #region Get base64 Key
            string base64Key = string.Empty;
            using (var ks = new SharedSecretService())
            {
                var secret = SecretFactory.Create();
                base64Key = await ks.CreateKeyAsync(secret);
            }
            #endregion

            #region Create key object
            keyMeta.ExpireOn = this.GetExpireOn(this.appSettings?.Kms?.KeyVault?.SharedSecret?.DefaultKeyExpire);
            CipherKey finalKey = await this.GenCipherKeyAsync(KeyTypeEnum.SharedSecret, keyMeta, base64Key);
            #endregion

            #region Save key to KeyVault
            await this.SaveKeyAsync(finalKey.Id, finalKey);
            #endregion

            return await Task.FromResult(finalKey);
        }

        /// <summary>
        /// Create a new RSA key-pair
        /// </summary>
        /// <param name="keyMeta">Key metadata</param>
        /// <returns>New key-pair</returns>
        /// <remarks>For Receiver/Sender</remarks>
        public async Task<CipherKey> CreateRsaAsync(KeyMetadata keyMeta)
        {
            if (keyMeta == null || keyMeta?.Owner == null)
            {
                throw new ArgumentNullException($"{nameof(KeyMetadata)} or Owner");
            }

            #region Get base64 Key
            IList<string> base64Keypair = null;
            using (var rsa = new RsaService())
            {
                base64Keypair = await rsa.CreateKeyPairAsync();

                if (base64Keypair == null || base64Keypair.Count != 2)
                {
                    throw new Exception($"Create RSA key-pair fails on CreateRsaAsync!");
                }
            }
            #endregion

            #region Create key object
            var privateKey = base64Keypair[0];
            var publicKey = base64Keypair[1];
            keyMeta.ExpireOn = this.GetExpireOn(this.appSettings?.Kms?.KeyVault?.Asymmetric?.DefaultKeyExpire);
            CipherKey key = await this.GenCipherKeyAsync(KeyTypeEnum.Rsa, keyMeta, publicKey, privateKey);
            #endregion

            #region Save key to KeyVault
            await this.SaveKeyAsync(key.Id, key);
            #endregion

            return await Task.FromResult(key);
        }

        /// <summary>
        /// Update exist key
        /// </summary>
        /// <param name="key">CipherKey</param>
        public async Task UpdateAsync(CipherKey key)
        {
            await this.SaveKeyAsync(key.Id, key, isForceSetExist: true);
        }

        /// <summary>
        /// Revoke a key by key id
        /// </summary>
        /// <param name="keyId">Key Id</param>
        public async Task RevokeAsync(string keyId)
        {
            var value = await this.redis.Database.HashGetAsync(RedisKeyFactory.KeyVault, hashField: keyId);
            var k = JsonConvert.DeserializeObject<CipherKey>(value);
            this.RevokeKey(k);
            _ = await this.redis.Database.HashSetAsync(RedisKeyFactory.KeyVault, k.Id, JsonConvert.SerializeObject(k));
        }

        /// <summary>
        /// Revoke all keys
        /// </summary>
        public async Task RevokeAllAsync()
        {
            // Get all keys
            var keys = await this.GetAllKeysFromHash();

            // Search key
            keys.ForEach(k =>
            {
                if (!k.IsDeprecated)
                {
                    this.RevokeKey(k);

                    var value = JsonConvert.SerializeObject(k);
                    this.redis.Database.HashSetAsync(RedisKeyFactory.KeyVault, k.Id, value);
                }
            });
        }

        /// <summary>
        /// Remove a key by key id
        /// </summary>
        /// <param name="keyId">Key Id</param>
        public async Task RemoveAsync(string keyId)
        {
            _ = await this.redis.Database.HashDeleteAsync(RedisKeyFactory.KeyVault, hashField: keyId);
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns>Cipher key readonly collection</returns>
        public async Task<IReadOnlyCollection<CipherKey>> GetAllAsync()
        {
            // Get all keys
            var keys = await this.GetAllKeysFromHash();

            // Search key
            return keys.AsEnumerable().ToList().AsReadOnly();
        }

        /// <summary>
        /// Find a key by Key Id
        /// </summary>
        /// <param name="keyId">Key Id</param>
        /// <returns>Key information</returns>
        public async Task<CipherKey> FindAsync(string keyId)
        {
            var value = await this.redis.Database.HashGetAsync(RedisKeyFactory.KeyVault, hashField: keyId);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<CipherKey>(value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Find from all keys
        /// </summary>
        /// <returns>Cipher key readonly collection</returns>
        public async Task<IReadOnlyCollection<CipherKey>> FindAsync(Func<CipherKey, bool> expression)
        {
            // Get all keys
            var keys = await this.GetAllKeysFromHash();

            // Search key
            return keys.AsEnumerable().Where(expression).ToList().AsReadOnly();
        }

        /// <summary>
        /// Backup a deprecated key
        /// </summary>
        public async Task BackupAsync(CipherKey deprecatedKey)
        {
            _ = await this.redis.Database.HashSetAsync(RedisKeyFactory.KeyVaultDeprecated, deprecatedKey.Id, JsonConvert.SerializeObject(deprecatedKey));
        }

        /// <summary>
        /// Get all keys and deserialize them
        /// </summary>
        /// <returns>List of keys</returns>
        private async Task<List<CipherKey>> GetAllKeysFromHash()
        {
            // Get all keys
            var keys = new List<CipherKey>();
            HashEntry[] hashEntries = await this.redis.Database.HashGetAllAsync(RedisKeyFactory.KeyVault);
            hashEntries.ToList().ForEach(h => keys.Add(JsonConvert.DeserializeObject<CipherKey>(h.Value)));
            return keys;
        }

        /// <summary>
        /// Create the final key object (Cipherkey)
        /// </summary>
        /// <param name="keyMeta">Key's metadata</param>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<CipherKey> GenCipherKeyAsync(KeyTypeEnum keyType, KeyMetadata keyMeta, string key1, string key2 = "")
        {
            var now = DateTimeOffset.Now;
            var keyId = Guid.NewGuid().ToString();
            var activeOn = keyMeta.ActiveOn ?? now;
            var expireOn = keyMeta.ExpireOn ?? activeOn.AddYears(DefaultKeyExpireYear);

            var finalKey = new CipherKey
            {
                Id = keyId,
                KeyType = keyType,
                CreateOn = now.ToProtobufTimestamp(),
                ActiveOn = activeOn.ToProtobufTimestamp(),
                ExpireOn = expireOn.ToProtobufTimestamp(),
                Key1 = key1,
                Key2 = key2,
                Purpose = keyMeta.Purpose ?? string.Empty,
                Expando = keyMeta.Expando == null ? null : new Google.Protobuf.WellKnownTypes.Any
                {
                    Value = Google.Protobuf.ByteString.CopyFrom(ByteStringUtils.ToByteArray(keyMeta.Expando))
                },
                Owner = new CipherKeyOwner
                {
                    Name = keyMeta.Owner.Name,
                    Host = keyMeta.Owner.Host
                }
            };

            return await Task.FromResult(finalKey);
        }

        /// <summary>
        /// Save key to KeyVault
        /// </summary>
        /// <param name="keyId">Key ID</param>
        /// <param name="key">Key</param>
        /// <param name="isForceSetExist">Enable force-set mode, default: false(no)</param>
        private async Task SaveKeyAsync(string keyId, CipherKey key, bool isForceSetExist = false)
        {
            if (isForceSetExist)
            {
                _ = await this.redis.Database.HashSetAsync(RedisKeyFactory.KeyVault, keyId, JsonConvert.SerializeObject(key));
            }
            else
            {
                var isFieldExistInHash = await this.redis.Database.HashExistsAsync(RedisKeyFactory.KeyVault, keyId);
                if (!isFieldExistInHash)
                {
                    _ = await this.redis.Database.HashSetAsync(RedisKeyFactory.KeyVault, keyId, JsonConvert.SerializeObject(key));
                }
                else
                {
                    throw new Exception($"Field {keyId} already existed in Hash key: {RedisKeyFactory.KeyVault}, cannot overwrite it in non-force-set mode");
                }
            }
        }

        /// <summary>
        /// Revoke a key
        /// </summary>
        /// <param name="key">Key</param>
        private void RevokeKey(CipherKey key)
        {
            key.RevokeOn = DateTimeOffset.Now.ToProtobufTimestamp();
            key.IsDeprecated = true;
        }

        /// <summary>
        /// Get ExpireOn by configuration
        /// </summary>
        /// <param name="keyExpireOption">DefaultKeyExpireOption</param>
        /// <returns>Expire on</returns>
        private DateTime GetExpireOn(DefaultKeyExpireOptions keyExpireOption)
        {
            var expireOn = DateTime.Now;

            if (keyExpireOption.Year.HasValue)
            {
                expireOn = expireOn.AddYears(keyExpireOption.Year.Value);
            }
            else if (keyExpireOption.Month.HasValue)
            {
                expireOn = expireOn.AddMonths(keyExpireOption.Month.Value);
            }
            else if (keyExpireOption.Day.HasValue)
            {
                expireOn = expireOn.AddDays(keyExpireOption.Day.Value);
            }
            else if (keyExpireOption.Hour.HasValue)
            {
                expireOn = expireOn.AddHours(keyExpireOption.Hour.Value);
            }
            else if (keyExpireOption.Minute.HasValue)
            {
                expireOn = expireOn.AddMinutes(keyExpireOption.Minute.Value);
            }
            else if (keyExpireOption.Second.HasValue)
            {
                expireOn = expireOn.AddSeconds(keyExpireOption.Second.Value);
            }

            return expireOn;
        }
    }
}
