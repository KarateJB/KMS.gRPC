using System.Threading.Tasks;
using Kms.Core;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// Interface for Asymmetric Crypto Service
    /// </summary>
    public interface IAsymCryptoService : IAsymKeyService
    {
        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        string SignSignature(CipherKey key, string data);

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        bool VerifySignature(CipherKey key, string data, string signature);

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        string Encrypt(CipherKey key, string data);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        string Decrypt(CipherKey key, string cipherData);

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        Task<string> SignSignatureAsync(CipherKey key, string data);

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        Task<bool> VerifySignatureAsync(CipherKey key, string data, string signature);

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        Task<string> EncryptAsync(CipherKey key, string data);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        Task<string> DecryptAsync(CipherKey key, string cipherData);
    }
}
