﻿using System.Threading.Tasks;

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
        /// Audit working keys
        /// </summary>
        Task AuditWorkingKeysAsync();

        /// <summary>
        /// Audit working keys
        /// </summary>
        Task AuditWorkingKeysBidAsync();

        /// <summary>
        /// Renew expire keys
        /// </summary>
        Task RenewKeysBidAsync();
    }
}