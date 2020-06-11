using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// Interface for Asymmetric Key Service
    /// </summary>
    public interface IAsymKeyService
    {
        /// <summary>
        /// Create Asymmetric Keys
        /// </summary>
        /// <returns>Key list as [private key, public key]</returns>
        IList<string> CreateKeyPair();

        /// <summary>
        /// Create Asymmetric Keys
        /// </summary>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        CipherKey CreateKeyPair(KeyMetadata meta);

        /// <summary>
        /// Create Asymmetric Keys
        /// </summary>
        /// <returns>Key list as [private key, public key]</returns>
        Task<IList<string>> CreateKeyPairAsync();

        /// <summary>
        /// Create Asymmetric Keys
        /// </summary>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        Task<CipherKey> CreateKeyPairAsync(KeyMetadata meta);
    }
}
