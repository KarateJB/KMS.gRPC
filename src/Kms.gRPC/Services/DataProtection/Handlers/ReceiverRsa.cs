using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.DataProtection.Handlers
{
    /// <summary>
    /// RSA receiver
    /// </summary>
    public class ReceiverRsa : IHandler
    {
        public IHandler Next { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        /// <param name="keyVault">KeyVault</param>
        /// <param name="keyType">Key type</param>
        /// <param name="keyMeta">Key's metadata</param>
        /// <returns>CipherKey object</returns>
        public async Task<CipherKey> ActionAsync(IKeyVault keyVault, KeyTypeEnum keyType, KeyMetadata keyMeta)
        {
            CipherKey key = default(CipherKey);
            if (keyType.Equals(KeyTypeEnum.Rsa))
            {
                key = await keyVault.CreateRsaAsync(keyMeta);
                return key;
            }
            else
            {
                this.Next = new ReceiverNotSupport();
                return await this.Next.ActionAsync(keyVault, keyType, keyMeta);
            }
        }
    }
}
