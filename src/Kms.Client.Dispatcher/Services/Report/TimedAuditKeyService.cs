using System;
using System.Reactive.Linq;
using System.Threading;
using Kms.Core.Models.Config.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kms.gRPC.Client.Services.Report
{
    /// <summary>
    /// Timer service for report notify
    /// </summary>
    public class TimedAuditKeyService : IDisposable
    {
        private const int DefaultReportNotifyTime = 600;
        private readonly AppSettings appSettings = null;
        private readonly ILogger logger = null;
        private readonly Timer timer = null;
        private IDisposable subscription = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">AppSetting options</param>
        /// <param name="logger">Logger</param>
        public TimedAuditKeyService(
            IOptions<AppSettings> configuration,
            ILogger<TimedAuditKeyService> logger)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;

            // Get timer's trigger timing
            var reportNotifyPeriod = this.appSettings?.KmsClient?.ReportNotifyTime ?? DefaultReportNotifyTime;

            // Logging
            this.logger.LogDebug($"KMS's notify-report-timer service will trigger every {reportNotifyPeriod.ToString()} seconds");

            // Set timer
            // this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(reportNotifyPeriod));

            // Reactive
            subscription = Observable.Interval(TimeSpan.FromSeconds(reportNotifyPeriod)).Subscribe(x => this.InvokeAuditKeyCallback());
        }

        /// <summary>
        /// Event handler
        /// </summary>
        public event EventHandler<AuditKeyEventArgs> ReportNotify;

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
            this.InvokeAuditKeyCallback();
        }

        private void InvokeAuditKeyCallback()
        {
            this.logger.LogDebug($"Start report working keys...");

            var eventArgs = new AuditKeyEventArgs();
            this.ReportNotify?.Invoke(this, eventArgs);
        }
    }
}
