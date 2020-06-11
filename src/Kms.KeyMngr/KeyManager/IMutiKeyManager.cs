using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Core;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// Interface for Multiple keys Manager
    /// </summary>
    public interface IMultiKeyClientManager
    {
        /// <summary>
        /// Get current working keys
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>CipherKey collection</returns>
        Task<IReadOnlyCollection<CipherKey>> GetKeysAsync(KeyTypeEnum keyType);

        /// <summary>
        /// Get current working keys
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="filter">Filter</param>
        /// <returns>CipherKey</returns>
        Task<CipherKey> FindKeyAsync(KeyTypeEnum keyType, Func<CipherKey, bool> filter);

        /// <summary>
        /// Save/Overwrite mutiple keys to the Key type (Overwrite all by Key type)
        /// </summary>
        /// <param name="keyType">KeyTypeEnum</param>
        /// <param name="keys">IReadOnlyCollection of CipherKey</param>
        Task SaveKeysAsync(KeyTypeEnum keyType, IReadOnlyCollection<CipherKey> keys);

        /// <summary>
        /// Save/Update single key in keys (Overwrite by Key type + Owner'name, or insert if not exist)
        /// </summary>
        /// <param name="keyType">KeyTypeEnum</param>
        /// <param name="key">The key to update</param>
        /// <param name="filter">Filter</param>
        Task UpdateKeysAsync(KeyTypeEnum keyType, CipherKey key, Func<CipherKey, bool> filter = null);
    }
}
