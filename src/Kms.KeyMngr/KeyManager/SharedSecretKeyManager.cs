using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core;
using Kms.KeyMngr.Utils.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// Shared secrey Key Manager
    /// </summary>
    public class SharedSecretKeyManager : BaseKeyManager, IKeyManager, IMultiKeyClientManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="memoryCache">MemoryCache</param>
        public SharedSecretKeyManager(
            ILogger<TripleDesKeyManager> logger,
            IMemoryCache memoryCache)
            : base(logger, memoryCache)
        {
        }

        /// <summary>
        /// Get current working Shared secrets
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>Shared secrets</returns>
        /// <remarks>This method is for Auth Server, which will store multiple Shared secrets</remarks>
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
        /// Save/Overwrite Shared secrets
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="keys">Shared secrets</param>
        /// <remarks>This method is for Auth Server, which will store multiple Shared secrets</remarks>
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
        /// <remarks>This method is for Auth Server, which will store multiple Shared secrets</remarks>
        public async Task UpdateKeysAsync(KeyTypeEnum keyType, CipherKey key, Func<CipherKey, bool> filter = null)
        {
            if (filter == null)
            {
                var str = (key.Expando as dynamic)?.ClientId.ToString();
                filter = await this.GetDefaultFilter(keyType, key.Owner?.Name, (key.Expando as dynamic)?.ClientId.ToString());
            }

            await this.UpdateKeysInMemoryCacheAsync(keyType, key, filter);
        }

        /// <summary>
        /// Get default filter
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="owner">Owner name</param>
        /// <param name="authClientId">Client id (Authentication)</param>
        /// <returns>Filter</returns>
        public async Task<Func<CipherKey, bool>> GetDefaultFilter(
            KeyTypeEnum keyType, string owner, string authClientId)
        {
            Func<CipherKey, bool> filter = null;
            var expandoFilters = new Dictionary<string, string>
                {
                    { "ClientId", authClientId }
                };

            filter = x => x.KeyType.Equals(keyType) && x.Owner.Name.Equals(owner) && x.Expando.MatchFilters(expandoFilters);

            return await Task.FromResult(filter);
        }
    }
}
