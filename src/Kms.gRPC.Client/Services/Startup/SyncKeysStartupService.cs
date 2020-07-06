using System.Threading.Tasks;
using Kms.Client.Dispatcher.Services;
using Kms.Core.Mock;
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
            var me = MockClients.Me;

            // Symmetric key
            await this.keyDispatcher.CreateSymmetricKeyAsync(me);

            // Shared secret
            await this.keyDispatcher.CreateSharedSecretsAsync(me);

            // Asymmetric key
            await this.keyDispatcher.CreateAsymmetricKeyAsync(me);

            // Reporing working keys
            // await this.keyDispatcher.AuditWorkingKeysAsync(); // Client streaming
            // await this.keyDispatcher.AuditWorkingKeysBidAsync(); // Bidirectional streaming
        }
    }
}
