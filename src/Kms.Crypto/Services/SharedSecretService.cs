using System;
using System.Text;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using Kms.Crypto.Utils;
using static Kms.Core.CipherKey.Types;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// Shared secret Service
    /// </summary>
    public class SharedSecretService : IKeyService, IDisposable
    {
        private const int DefaultKeyExpireYear = 1;

        /// <summary>
        /// Constructor
        /// </summary>
        public SharedSecretService()
        {
        }

        /// <summary>
        /// Create Shared secret
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <returns>Shared secret</returns>
        public string CreateKey(string secret)
        {
            byte[] keyArray = null;
            using (var md5Serv = System.Security.Cryptography.MD5.Create())
            {
                keyArray = md5Serv.ComputeHash(UTF8Encoding.Unicode.GetBytes(secret)); // 16 bytes
            }

            return Convert.ToBase64String(keyArray);
        }

        /// <summary>
        /// Create Shared secret
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        public CipherKey CreateKey(string secret, KeyMetadata meta)
        {
            var base64Key = this.CreateKey(secret);
            var key = CipherKeyUtils.Create(KeyTypeEnum.SharedSecret, base64Key, meta);
            return key;
        }

        /// <summary>
        /// Create Shared secret
        /// </summary>
        /// <param name="secret">Base secret</param>
        /// <returns>Shared secret</returns>
        public async Task<string> CreateKeyAsync(string secret)
        {
            var key = await Task.Run(() => this.CreateKey(secret));
            return key;
        }

        /// <summary>
        /// Create Shared secret
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
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}
