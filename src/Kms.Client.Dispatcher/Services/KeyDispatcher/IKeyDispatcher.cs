using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Core;

namespace Kms.Client.Dispatcher.Services
{
    /// <summary>
    /// Interface of KeyDispatcher
    /// </summary>
    public interface IKeyDispatcher
    {
        /// <summary>
        /// Create and sync an asymmetric key-pair
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>true(OK)/false(NG)</returns>
        Task<bool> CreateAsymmetricKeyAsync(string client);

        /// <summary>
        /// Create and sync shared secrets
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>true(OK)/false(NG)</returns>
        Task<bool> CreateSharedSecretsAsync(string client);

        /// <summary>
        /// Sync first symmetric key
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>true(OK)/false(NG)</returns>
        Task<bool> CreateSymmetricKeyAsync(string client);

        /// <summary>
        /// Get shared secret(s) of certain client
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>Shared secrets</returns>
        Task<IReadOnlyCollection<CipherKey>> GetSharedSecretsAsync(string client);

        /// <summary>
        /// Get recievers' public keys
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="receiver">Receiver</param>
        /// <returns>Public keys</returns>
        Task<IReadOnlyCollection<CipherKey>> GetPublicKeysAsync(string client, IList<string> receivers);

        /// <summary>
        /// Audit working keys
        /// </summary>
        /// <param name="client">Client</param>
        Task AuditWorkingKeysAsync(string client);

        /// <summary>
        /// Audit working keys
        /// </summary>
        /// <param name="client">Client</param>
        Task AuditWorkingKeysBidAsync(string client);

        /// <summary>
        /// Renew expire keys
        /// </summary>
        /// <param name="client">Client</param>
        Task RenewKeysBidAsync(string client);
    }
}
