using System.Threading.Tasks;
using Kms.Core;
using static Kms.Core.CipherKey.Types;

namespace Kms.KeyMngr.KeyManager
{
    /// <summary>
    /// Interface for Asymmetric Key Manager
    /// </summary>
    public interface IAsymKeyManager
    {
        /// <summary>
        /// Create a default RSA public key
        /// </summary>
        /// <param name="receiver">The target receiver</param>
        /// <param name="isIncludePrivateKey">Is the key pair should include Private key (False for a sender, True for a receiver)</param>
        /// <returns>CipherKey object</returns>
        /// <remarks>This method will be used when cannot get receiver's public key from KMS</remarks>
        Task<CipherKey> CreateDefaultAsymmetricKey(string receiver, bool isIncludePrivateKey = true);

        /// <summary>
        /// Get my private key
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="me">Me</param>
        /// <returns>Private key</returns>
        Task<string> GetPrivateKeyAsync(KeyTypeEnum keyType, string me);

        /// <summary>
        /// Get receiver's public key
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="receiver">Receiver</param>
        /// <returns>Public key</returns>
        Task<string> GetPublicKeyAsync(KeyTypeEnum keyType, string receiver);
    }
}
