using System;
using System.Reactive.Linq;
using Kms.Core.Models.Config.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kms.Client.Dispatcher.Services
{
    /// <summary>
    /// Timer service for renewing expired key
    /// </summary>
    public class TimedRenewKeysService : IDisposable
    {
        private const int DefaultCheckKeyTime = 3600;
        private readonly AppSettings appSettings = null;
        private readonly ILogger logger = null;
        private IDisposable subscription = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">AppSetting options</param>
        /// <param name="logger">Logger</param>
        /// <param name="keyVault">KeyVault</param>
        public TimedRenewKeysService(
            IOptions<AppSettings> configuration,
            ILogger<TimedRenewKeysService> logger)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;

            // Get timer's trigger timing
            var checkKeyPeriod = this.appSettings?.KmsClient?.CheckKeyTime ?? DefaultCheckKeyTime;

            // Logging
            this.logger.LogDebug($"KMS client's renew-key-timer will trigger every {checkKeyPeriod.ToString()} seconds.");

            // Reactive
            subscription = Observable.Interval(TimeSpan.FromSeconds(checkKeyPeriod)).Subscribe(x => this.InvokeRenewKeysCallback());
        }

        /// <summary>
        /// Event handler
        /// </summary>
        public event EventHandler<RenewKeysEventArgs> RenewKeysEvents;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.subscription.Dispose();
        }

        private void InvokeRenewKeysCallback()
        {
            this.logger.LogDebug($"Invoke renewing expired keys...");

            var eventArgs = new RenewKeysEventArgs();
            this.RenewKeysEvents?.Invoke(this, eventArgs);
        }
    }
}
