using System;
using System.Reactive.Linq;
using System.Threading;
using Kms.Core.Models.Config.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kms.Client.Dispatcher.Services
{
    /// <summary>
    /// Timer service for report notify
    /// </summary>
    public class TimedAuditKeysService : IDisposable
    {
        private const int DefaultReportNotifyTime = 600;
        private readonly AppSettings appSettings = null;
        private readonly ILogger logger = null;
        // private readonly Timer timer = null;
        private IDisposable subscription = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">AppSetting options</param>
        /// <param name="logger">Logger</param>
        public TimedAuditKeysService(
            IOptions<AppSettings> configuration,
            ILogger<TimedAuditKeysService> logger)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;

            // Get timer's trigger timing
            var reportNotifyPeriod = this.appSettings?.KmsClient?.ReportNotifyTime ?? DefaultReportNotifyTime;

            // Logging
            this.logger.LogDebug($"KMS client's notify-report-timer will trigger every {reportNotifyPeriod.ToString()} seconds.");

            // Set timer
            // this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(reportNotifyPeriod));

            // Reactive
            subscription = Observable.Interval(TimeSpan.FromSeconds(reportNotifyPeriod)).Subscribe(x => this.InvokeAuditKeysCallback());
        }

        /// <summary>
        /// Event handler
        /// </summary>
        public event EventHandler<AuditKeysEventArgs> AuditKeysEvents;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // this.timer?.Change(Timeout.Infinite, 0);
            // this.timer?.Dispose();
            this.subscription.Dispose();
        }

        private void DoWork(object state)
        {
            this.InvokeAuditKeysCallback();
        }

        private void InvokeAuditKeysCallback()
        {
            this.logger.LogDebug($"Invoke auditing working keys...");

            var eventArgs = new AuditKeysEventArgs();
            this.AuditKeysEvents?.Invoke(this, eventArgs);
        }
    }
}
