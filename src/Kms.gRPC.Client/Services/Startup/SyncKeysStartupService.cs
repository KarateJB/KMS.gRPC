using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kms.gRPC.Client.Dispatcher;
using Microsoft.Extensions.Logging;

namespace Kms.gRPC.Client.Services.Startup
{
    /// <summary>
    /// Sync keys startup service
    /// </summary>
    public class SyncKeysStartupService : IStartupService
    {
        private readonly ILogger logger = null;
        private readonly IKeyDispatcher keyDispatcher = null;

        public SyncKeysStartupService(
            ILogger<SyncKeysStartupService> logger,
            IKeyDispatcher keyDispatcher)
        {
            this.logger = logger;
            this.keyDispatcher = keyDispatcher;
        }

        /// <summary>
        /// Priority
        /// </summary>
        public int Priority { get; set; } = 1;

        /// <summary>
        /// Start
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            // Symmetric key
            await this.keyDispatcher.CreateSymmetricKeyAsync();

            // Shared secret
            await this.keyDispatcher.CreateSharedSecretsAsync();

            // Asymmetric key
            await this.keyDispatcher.CreateAsymmetricKeyAsync();

            // Reporing working keys
            // await this.keyDispatcher.AuditWorkingKeysAsync(); // Client streaming
            // await this.keyDispatcher.AuditWorkingKeysBidAsync(); // Bidirectional streaming
        }
    }
}
