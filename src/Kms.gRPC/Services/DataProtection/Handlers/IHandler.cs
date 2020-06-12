using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.DataProtection.Handlers
{
    /// <summary>
    /// IHandler
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Next handler
        /// </summary>
        IHandler Next { get; }

        /// <summary>
        /// Action
        /// </summary>
        /// <param name="keyVault">KeyVault</param>
        /// <param name="keyType">Key type</param>
        /// <param name="keyMeta">Key's metadata</param>
        /// <returns>CipherKey object</returns>
        Task<CipherKey> ActionAsync(IKeyVault keyVault, KeyTypeEnum keyType, KeyMetadata keyMeta);
    }
}
