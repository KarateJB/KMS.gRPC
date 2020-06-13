using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.DataProtection.Handlers
{
    /// <summary>
    /// TripleDES receiver
    /// </summary>
    public class ReceiverTripleDes : IHandler
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
            if (keyType.Equals(KeyTypeEnum.TripleDes))
            {
                key = await keyVault.CreateTripleDesAsync(keyMeta);
                return key;
            }
            else
            {
                this.Next = new ReceiverSharedSecret();
                return await this.Next.ActionAsync(keyVault, keyType, keyMeta);
            }
        }
    }
}
