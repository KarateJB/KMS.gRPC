using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;

namespace Kms.gRPC.Services.DataProtection
{
    /// <summary>
    /// Key Vault interface
    /// </summary>
    public interface IKeyVault
    {
        /// <summary>
        /// Create a new TripleDES key
        /// </summary>
        /// <param name="keyMeta">Key metadata</param>
        /// <returns>New key</returns>
        Task<CipherKey> CreateTripleDesAsync(KeyMetadata keyMeta);

        /// <summary>
        /// Create a new Secret key
        /// </summary>
        /// <param name="keyMeta">Key metadata</param>
        /// <returns>New key</returns>
        /// <remarks>For Auth server and its clients</remarks>
        Task<CipherKey> CreateSharedSecretAsync(KeyMetadata keyMeta);

        /// <summary>
        /// Create a new RSA key-pair
        /// </summary>
        /// <param name="keyMeta">Key metadata</param>
        /// <returns>New key-pair</returns>
        /// <remarks>For Receiver/Sender</remarks>
        Task<CipherKey> CreateRsaAsync(KeyMetadata keyMeta);

        /// <summary>
        /// Update exist key
        /// </summary>
        /// <param name="key">CipherKey</param>
        Task UpdateAsync(CipherKey key);

        /// <summary>
        /// Revoke a key by key id
        /// </summary>
        /// <param name="keyId">Key Id</param>
        Task RevokeAsync(string keyId);

        /// <summary>
        /// Revoke all keys
        /// </summary>
        Task RevokeAllAsync();

        /// <summary>
        /// Remove a key by key id
        /// </summary>
        /// <param name="keyId">Key Id</param>
        Task RemoveAsync(string keyId);

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns>Cipher key readonly collection</returns>
        Task<IReadOnlyCollection<CipherKey>> GetAllAsync();

        /// <summary>
        /// Find a key by Key Id
        /// </summary>
        /// <param name="keyId">Key Id</param>
        /// <returns>Key information</returns>
        Task<CipherKey> FindAsync(string keyId);

        /// <summary>
        /// Find from all keys
        /// </summary>
        /// <returns>Cipher key readonly collection</returns>
        Task<IReadOnlyCollection<CipherKey>> FindAsync(Func<CipherKey, bool> expression);

        /// <summary>
        /// Backup a deprecated key
        /// </summary>
        Task BackupAsync(CipherKey deprecatedKey);
    }
}
