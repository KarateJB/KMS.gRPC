using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Core.Utils.Extensions;
using Kms.KeyMngr.Factory;
using Kms.KeyMngr.Utils.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// Base Key Manager
    /// </summary>
    public class BaseKeyManager
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger logger = null;

        /// <summary>
        /// MemoryCache
        /// </summary>
        protected readonly IMemoryCache memoryCache = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="memoryCache">MemoryCache</param>
        public BaseKeyManager(
            ILogger<TripleDesKeyManager> logger,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Show all saved keys
        /// </summary>
        protected virtual async Task ShowAllSavedKeys()
        {
            string pattern = $"^{CacheKeyFactory.KeyPrefixCipher}(.+)";
            _ = this.memoryCache.GetAllCacheEntries(pattern, this.logger);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Set key(s) to memory cache
        /// </summary>
        /// <typeparam name="T">Object</typeparam>
        /// <param name="keyType">Key type</param>
        /// <param name="eitherKeyOrKeys">Key or keys</param>
        protected virtual async Task SetToMemoryCacheAsync<T>(KeyTypeEnum keyType, T eitherKeyOrKeys)
        {
            if (eitherKeyOrKeys == null)
            {
                this.logger.CustomLogWarn("Wont save key cus it's empty!");
                return;
            }

            this.memoryCache.Set(CacheKeyFactory.GetKeyCipher(keyType), eitherKeyOrKeys);
            this.logger.CustomLogDebug($"Successfully set(overwrite) {keyType.ToString()} key(s) to memory cache.");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get single key from memory cache
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>Share secret</returns>
        protected virtual async Task<CipherKey> GetKeyFromMemoryCacheAsync(KeyTypeEnum keyType)
        {
            this.logger.CustomLogDebug($"Start loading {keyType.ToString()} key from MemoryCache.");

            try
            {
                var key = this.memoryCache.Get<CipherKey>(CacheKeyFactory.GetKeyCipher(keyType));

                this.logger.CustomLogDebug($"Successfully load {keyType.ToString()} key from MemoryCache. {key.ToString()}");
                return await Task.FromResult(key);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{this.GetType().ToString()} error");
                await this.ShowAllSavedKeys();
                return null;
            }
        }

        /// <summary>
        /// Get current working keys
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>Shared secrets</returns>
        /// <remarks>This method is for Auth Server, which will store multiple Shared secrets</remarks>
        protected virtual async Task<IReadOnlyCollection<CipherKey>> GetKeysFromMemoryCacheAsync(KeyTypeEnum keyType)
        {
            this.logger.CustomLogDebug($"Start load {keyType.ToString()} keys from MemoryCache...");

            try
            {
                var keys = this.memoryCache.Get<IReadOnlyCollection<CipherKey>>(CacheKeyFactory.GetKeyCipher(keyType));

                this.logger.CustomLogDebug($"Successfully load {keyType.ToString()} keys from MemoryCache.");
                return await Task.FromResult(keys);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{this.GetType().ToString()} error");
                await this.ShowAllSavedKeys();
                throw ex;
            }
        }

        /// <summary>
        /// Save/Update single key in keys (Overwrite by Key type + Owner'name, or insert if not exist)
        /// </summary>
        /// <param name="keyType">KeyTypeEnum</param>
        /// <param name="key">The key to update</param>
        /// <param name="filter">Filter</param>
        /// <remarks>This method is for Auth Server, which will store multiple Shared secrets</remarks>
        protected virtual async Task UpdateKeysInMemoryCacheAsync(
            KeyTypeEnum keyType,
            CipherKey key,
            Func<CipherKey, bool> filter)
        {
            IList<CipherKey> keys = null;

            try
            {
                if (key != null)
                {
                    // The origin keys
                    var rokeys = await this.GetKeysFromMemoryCacheAsync(keyType);
                    keys = rokeys == null ? new List<CipherKey>() : rokeys.ToList();

                    // Delete the matched key(s) by key type + owner's name
                    var oldKeys = keys.Where(filter).ToList();
                    oldKeys.ForEach(o => keys.Remove(o));

                    // Add the latest key
                    keys.Add(key);

                    // Save to Memory Cache
                    await this.SetToMemoryCacheAsync(keyType, keys);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{this.GetType().ToString()} error");
                throw;
            }
        }
    }
}
