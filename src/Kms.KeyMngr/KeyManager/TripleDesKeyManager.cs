using System.Threading.Tasks;
using Kms.Core;
using Kms.KeyMngr.KeyManager;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// TripleDES Key Manager
    /// </summary>
    public class TripleDesKeyManager : BaseKeyManager, IKeyManager, ISingleKeyClientManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="memoryCache">MemoryCache</param>
        public TripleDesKeyManager(
            ILogger<TripleDesKeyManager> logger,
            IMemoryCache memoryCache)
            : base(logger, memoryCache)
        {
        }

        /// <summary>
        /// Get TripleDES key
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>TripleDES key</returns>
        public async Task<CipherKey> GetKeyAsync(KeyTypeEnum keyType)
        {
            var key = await this.GetKeyFromMemoryCacheAsync(keyType);
            return key;
        }

        /// <summary>
        /// Save/Overwrite a TripleDES key (Overwrite by Key type)
        /// </summary>
        /// <param name="key">Key</param>
        public async Task SaveKeyAsync(CipherKey key)
        {
            await this.SetToMemoryCacheAsync(key.KeyType, key);
        }
    }
}
