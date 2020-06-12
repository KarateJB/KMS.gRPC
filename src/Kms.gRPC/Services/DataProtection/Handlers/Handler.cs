using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.DataProtection.Handlers
{
    /// <summary>
    /// Handler
    /// </summary>
    public class Handler : IHandler
    {
        /// <summary>
        /// Next handler
        /// </summary>
        public virtual IHandler Next { get; set; } = new ReceiverTripleDes();

        /// <summary>
        /// Action
        /// </summary>
        /// <param name="keyVault">KeyVault</param>
        /// <param name="keyType">Key type</param>
        /// <param name="keyMeta">Key's metadata</param>
        /// <returns>CipherKey object</returns>
        public virtual async Task<CipherKey> ActionAsync(IKeyVault keyVault, KeyTypeEnum keyType, KeyMetadata keyMeta)
        {
            return await this.Next.ActionAsync(keyVault, keyType, keyMeta);
        }
    }
}
