using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Core.Utils.Extensions;
using Kms.Crypto.Services;
using Kms.KeyMngr.Factory;
using Kms.KeyMngr.KeyManager;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// RSA Key Manager
    /// </summary>
    public class RsaKeyManager : BaseKeyManager, IKeyManager, IMultiKeyClientManager, IAsymKeyManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="memoryCache">MemoryCache</param>
        public RsaKeyManager(
            ILogger<TripleDesKeyManager> logger,
            IMemoryCache memoryCache)
            : base(logger, memoryCache)
        {
        }

        /// <summary>
        /// Get current working RSA key-pairs
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>RSA key-pairs</returns>
        /// <remarks>
        /// This method is for all clients, which will store multiple RSA key-pairs to exchange data.
        /// </remarks>
        public async Task<IReadOnlyCollection<CipherKey>> GetKeysAsync(KeyTypeEnum keyType)
        {
            var keys = await this.GetKeysFromMemoryCacheAsync(keyType);
            return keys;
        }

        /// <summary>
        /// Get current working keys
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="filter">Filter</param>
        /// <returns>CipherKey</returns>
        public async Task<CipherKey> FindKeyAsync(KeyTypeEnum keyType, Func<CipherKey, bool> filter)
        {
            var keys = await this.GetKeysFromMemoryCacheAsync(keyType);
            return keys == null ? null : keys.ToList().FirstOrDefault(filter);
        }

        /// <summary>
        /// Save/Overwrite RSA key-pairs
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="keys">RSA key-pairs</param>
        /// <remarks>
        /// This method is for all clients, which will store multiple RSA key-pairs to exchange data.
        /// </remarks>
        public async Task SaveKeysAsync(KeyTypeEnum keyType, IReadOnlyCollection<CipherKey> keys)
        {
            await this.SetToMemoryCacheAsync(keyType, keys);
        }

        /// <summary>
        /// Save/Update single key in keys (Overwrite by Key type + Owner'name, or insert if not exist)
        /// </summary>
        /// <param name="keyType">KeyTypeEnum</param>
        /// <param name="key">The key to update</param>
        /// <param name="filter">Filter</param>
        /// <remarks>
        /// This method is for all clients, which will store multiple RSA key-pairs to exchange data.
        /// </remarks>
        public async Task UpdateKeysAsync(KeyTypeEnum keyType, CipherKey key, Func<CipherKey, bool> filter = null)
        {
            if (filter == null)
            {
                filter = x => x.KeyType.Equals(key.KeyType) && x.Owner.Name.Equals(key.Owner.Name);
            }

            await this.UpdateKeysInMemoryCacheAsync(keyType, key, filter);
        }

        /// <summary>
        /// Create a default RSA public key
        /// </summary>
        /// <param name="receiver">The target receiver</param>
        /// <param name="isIncludePrivateKey">Is the key pair should include Private key (False for a sender, True for a receiver)</param>
        /// <returns>RSA key-pair</returns>
        public async Task<CipherKey> CreateDefaultAsymmetricKey(string receiver, bool isIncludePrivateKey = true)
        {
            #region Get base64 Key
            IList<string> base64Keypair = null;
            using (var rsa = new RsaService())
            {
                base64Keypair = await rsa.CreateKeyPairAsync();

                if (base64Keypair == null || base64Keypair.Count != 2)
                {
                    throw new Exception($"Create RSA key-pair fails!");
                }
            }
            #endregion

            #region Create key object
            var privateKey = isIncludePrivateKey ? base64Keypair[0] : string.Empty;
            var publicKey = base64Keypair[1];
            var key = new CipherKey
            {
                Id = Guid.NewGuid().ToString(),
                KeyType = KeyTypeEnum.Rsa,
                CreateOn = DateTimeOffset.Now.ToProtobufTimestamp(),
                Key1 = publicKey,
                Key2 = privateKey,
                Purpose = "Default asymmetric key for encrypting exchange data",
                Owner = new CipherKeyOwner
                {
                    Name = receiver.ToString(),
                    Host = string.Empty
                }
            };
            #endregion

            return await Task.FromResult(key);
        }

        /// <summary>
        /// Get my private key
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="me">Me</param>
        /// <returns>Private key</returns>
        public async Task<string> GetPrivateKeyAsync(KeyTypeEnum keyType, string me)
        {
            this.logger.CustomLogDebug($"Start load {me.ToString()}'s {keyType.ToString()} private key from MemoryCache...");

            try
            {
                var keys = this.memoryCache.Get<IReadOnlyCollection<CipherKey>>(CacheKeyFactory.GetKeyCipher(keyType));

                if (keys != null)
                {
                    var key = keys.FirstOrDefault(x => x.Owner != null && x.Owner.Name.Equals(me.ToString()));

                    this.logger.CustomLogDebug($"Successfully load {me.ToString()}'s {keyType.ToString()} private key from MemoryCache. {key.ToString()}");
                    return await Task.FromResult(key?.Key2);
                }
                else
                {
                    this.logger.CustomLogWarn($"Failed to load {me.ToString()}'s {keyType.ToString()} private key from MemoryCache.");
                    throw new NullReferenceException($"No available {me.ToString()}'s {keyType.ToString()} private key");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{this.GetType().ToString()} error");
                await this.ShowAllSavedKeys();
                throw ex;
            }
        }

        /// <summary>
        /// Get receiver's public key
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="receiver">Receiver</param>
        /// <returns>Public key</returns>
        public async Task<string> GetPublicKeyAsync(KeyTypeEnum keyType, string receiver)
        {
            this.logger.CustomLogDebug($"Start load {receiver.ToString()}'s {keyType.ToString()} public key from MemoryCache...");

            try
            {
                var keys = this.memoryCache.Get<IReadOnlyCollection<CipherKey>>(CacheKeyFactory.GetKeyCipher(keyType));

                if (keys != null)
                {
                    var key = keys.FirstOrDefault(x => x.Owner.Name.Equals(receiver.ToString()));

                    this.logger.CustomLogDebug($"Successfully load {receiver.ToString()}'s {keyType.ToString()} public key from MemoryCache. {key.ToString()}");
                    return await Task.FromResult(key?.Key1);
                }
                else
                {
                    this.logger.CustomLogWarn($"Failed to load {receiver.ToString()}'s {keyType.ToString()} public key from MemoryCache.");
                    throw new NullReferenceException($"No available {receiver.ToString()}'s {keyType.ToString()} public key");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{this.GetType().ToString()} error");
                await this.ShowAllSavedKeys();
                throw;
            }
        }
    }
}
