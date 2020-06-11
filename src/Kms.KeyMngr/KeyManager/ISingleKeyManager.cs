using System.Threading.Tasks;
using Kms.Core;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// Interface for Single Key Manager
    /// </summary>
    /// <remarks>Scope: Symmetric key</remarks>
    public interface ISingleKeyClientManager
    {
        /// <summary>
        /// Get current working key
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <returns>CipherKey object</returns>
        Task<CipherKey> GetKeyAsync(KeyTypeEnum keyType);

        /// <summary>
        /// Save/Overwrite a key (Overwrite by Key type)
        /// </summary>
        /// <param name="key">CipherKey</param>
        Task SaveKeyAsync(CipherKey key);
    }
}
