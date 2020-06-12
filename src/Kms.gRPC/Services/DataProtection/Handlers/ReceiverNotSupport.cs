using System;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.DataProtection.Handlers
{
    /// <summary>
    /// TripleDES receiver
    /// </summary>
    public class ReceiverNotSupport : IHandler
    {
        /// <summary>
        /// Next handler
        /// </summary>
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
            var err = await Task.FromResult($"KeyVault has not implemented create key method for {keyType.ToString()}");
            throw new NotSupportedException(err);
        }
    }
}
