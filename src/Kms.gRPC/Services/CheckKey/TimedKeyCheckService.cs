using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Core.Models.Config.Server;
using Kms.gRPC.Services.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.CheckKey
{
    /// <summary>
    /// Timer service for checking key
    /// </summary>
    public class TimedKeyCheckService : IDisposable
    {
        private const int DefaultCheckKeyTime = 3600;
        private readonly AppSettings appSettings = null;
        private readonly ILogger logger = null;
        private readonly IKeyVault keyVault = null;
        private readonly Timer timer = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">AppSetting options</param>
        /// <param name="logger">Logger</param>
        /// <param name="keyVault">KeyVault</param>
        public TimedKeyCheckService(
            IOptions<AppSettings> configuration,
            ILogger<TimedKeyCheckService> logger,
            IKeyVault keyVault)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;
            this.keyVault = keyVault;

            // Get timer's trigger timing
            ////var checkkeyPeriod = this.appSettings?.Kms? ?? DefaultCheckKeyTime;
            var checkkeyPeriod = DefaultCheckKeyTime;

            // Logging
            this.logger.LogDebug($"KMS's check-key-timer service will trigger every {checkkeyPeriod.ToString()} seconds");

            // Set timer
            this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(checkkeyPeriod));
        }

        /// <summary>
        /// Event handler
        /// </summary>
        public event EventHandler<KeyCheckEventArgs> RenewKey;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.logger.LogWarning("KMS's check-key-timer service had been disposed and won't renew key util it restarts!");
            this.timer?.Change(Timeout.Infinite, 0);
            this.timer?.Dispose();
        }

        private void DoWork(object state)
        {
            this.logger.LogDebug($"Start checking all keys...");

            IReadOnlyCollection<CipherKey> deprecatedKeys = null;

            Task.Run(async () =>
            {
                var keyCollection = await this.keyVault.GetAllAsync();

                // Debug
                keyCollection.ToList().ForEach(k =>
                {
                    this.logger.LogDebug($"ExpireOn={k.ExpireOn}, Now={DateTime.Now}, IsDeprecated={k.ExpireOn.ToDateTimeOffset() <= DateTimeOffset.Now}");
                });

                // Deprecated keys = (Not shared secret) and ( deprecated or expired)
                deprecatedKeys = keyCollection.Where(x =>
                    x.KeyType != KeyTypeEnum.SharedSecret &&
                    (x.IsDeprecated || x.ExpireOn.ToDateTimeOffset() <= DateTimeOffset.Now)).ToList().AsReadOnly();
            }).Wait();

            if (deprecatedKeys == null || deprecatedKeys.Count() == 0)
            {
                this.logger.LogDebug($"All keys are alive.");
            }
            else
            {
                this.logger.LogDebug($"{deprecatedKeys.Count} keys are revoked or expired, invoke RenewKey event.");
                var eventArgs = new KeyCheckEventArgs { DeprecatedKeys = deprecatedKeys };
                this.RenewKey?.Invoke(this, eventArgs);
            }
        }
    }
}
