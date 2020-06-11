using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using Kms.Crypto.Utils;
using static Kms.Core.CipherKey.Types;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// TripleDesService
    /// </summary>
    public class TripleDesService : ICipherService, IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TripleDesService()
        {
        }

        /// <summary>
        /// Create TripleDES Key
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <returns>TripleDES Key</returns>
        public string CreateKey(string secret)
        {
            byte[] keyArray = null;
            using (var md5Serv = System.Security.Cryptography.MD5.Create())
            {
                keyArray = md5Serv.ComputeHash(UTF8Encoding.Unicode.GetBytes(secret)); // 16 bytes

                // Use k1-k2-k1 as the fianl key
                if (keyArray.Length == 16)
                {
                    byte[] tmp = new byte[24];
                    Buffer.BlockCopy(keyArray, 0, tmp, 0, keyArray.Length);
                    Buffer.BlockCopy(keyArray, 0, tmp, keyArray.Length, 8);
                    keyArray = tmp;
                }
            }

            return Convert.ToBase64String(keyArray);
        }

        /// <summary>
        /// Create TripleDES key
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        public CipherKey CreateKey(string secret, KeyMetadata meta)
        {
            var base64Key = this.CreateKey(secret);
            var key = CipherKeyUtils.Create(KeyTypeEnum.TripleDes, base64Key, meta);
            return key;
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="base64Key">Key</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted text</returns>
        public string Encrypt(string base64Key, string data)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(data);

            using (var tripleDES = TripleDES.Create())
            {
                var byteKey = Convert.FromBase64String(base64Key);
                tripleDES.Key = byteKey;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="base64Key">Key</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public string Decrypt(string base64Key, string cipherData)
        {
            byte[] inputArray = Convert.FromBase64String(cipherData);
            using (var tripleDES = TripleDES.Create())
            {
                var byteKey = Convert.FromBase64String(base64Key);
                tripleDES.Key = byteKey;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }

        /// <summary>
        /// Create TripleDES Key
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <returns>TripleDES Key</returns>
        public async Task<string> CreateKeyAsync(string secret)
        {
            var key = await Task.Run(() => this.CreateKey(secret));
            return key;
        }

        /// <summary>
        /// Create TripleDES key
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        public async Task<CipherKey> CreateKeyAsync(string secret, KeyMetadata meta)
        {
            var key = await Task.Run(() => this.CreateKey(secret, meta));
            return key;
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="base64Key">Key</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted text</returns>
        public async Task<string> EncryptAsync(string base64Key, string data)
        {
            var encryptedData = await Task.Run(() => this.Encrypt(base64Key, data));
            return encryptedData;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="base64Key">Key</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public async Task<string> DecryptAsync(string base64Key, string cipherData)
        {
            var decryptedData = await Task.Run(() => this.Decrypt(base64Key, cipherData));
            return decryptedData;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}
