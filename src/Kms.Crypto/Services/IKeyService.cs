using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// Interface for Key Service
    /// </summary>
    public interface IKeyService
    {
        /// <summary>
        /// Create key
        /// </summary>
        /// <param name="secret">Secret</param>
        /// <returns>Key(base64)</returns>
        string CreateKey(string secret);

        /// <summary>
        /// Create key
        /// </summary>
        /// <param name="secret">Secret key</param>
        /// <param name="meta">Metadata</param>
        /// <returns>CipherKey object</returns>
        CipherKey CreateKey(string secret, KeyMetadata meta);

        /// <summary>
        /// Create key
        /// </summary>
        /// <param name="secret">Secret</param>
        /// <returns>Key(base64)</returns>
        Task<string> CreateKeyAsync(string secret);

        /// <summary>
        /// Create key
        /// </summary>
        /// <param name="secret">Secret key</param>
        /// <param name="meta">Metadata</param>
        /// <returns>CipherKey object</returns>
        Task<CipherKey> CreateKeyAsync(string secret, KeyMetadata meta);
    }
}
