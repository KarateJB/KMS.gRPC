using System.Threading.Tasks;

namespace Kms.gRPC.Client.Dispatcher
{
    /// <summary>
    /// Interface of KeyDispatcher
    /// </summary>
    public interface IKeyDispatcher
    {
        /// <summary>
        /// Create and sync an asymmetric key-pair
        /// </summary>
        /// <returns>true(OK)/false(NG)</returns>
        Task<bool> CreateAsymmetricKeyAsync();

        /// <summary>
        /// Create and sync shared secrets
        /// </summary>
        /// <returns>true(OK)/false(NG)</returns>
        Task<bool> CreateSharedSecretsAsync();

        /// <summary>
        /// Sync first symmetric key
        /// </summary>
        /// <returns>true(OK)/false(NG)</returns>
        Task<bool> CreateSymmetricKeyAsync();

        /// <summary>
        /// Get reciever's public key
        /// </summary>
        /// <param name="receiver">Receiver</param>
        /// <returns>true(OK)/false(NG)</returns>
        // Task<bool> GetPublicKeyAsync(string receiver);

        /// <summary>
        /// Get shared secret(s) of certain client
        /// </summary>
        /// <returns>true(OK)/false(NG)</returns>
        // Task<bool> GetSharedSecretsAsync();

        /// <summary>
        /// Report working keys to server
        /// </summary>
        Task AuditWorkingKeysAsync();

        /// <summary>
        /// Report working keys to server
        /// </summary>
        Task AuditWorkingKeysBidAsync();
    }
}
