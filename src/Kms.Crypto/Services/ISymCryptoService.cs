using System.Threading.Tasks;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// Interface for Cipher Service
    /// </summary>
    public interface ICipherService : IKeyService
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="base64Key">Key(base64)</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        string Encrypt(string base64Key, string data);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="base64Key">Key(base64)</param>
        /// <param name="cipherData">Encrypted input</param>
        /// <returns>Decrypted data</returns>
        string Decrypt(string base64Key, string cipherData);

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="base64Key">Key(base64)</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        Task<string> EncryptAsync(string base64Key, string data);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="base64Key">Key(base64)</param>
        /// <param name="cipherData">Encrypted input</param>
        /// <returns>Decrypted data</returns>
        Task<string> DecryptAsync(string base64Key, string cipherData);
    }
}
