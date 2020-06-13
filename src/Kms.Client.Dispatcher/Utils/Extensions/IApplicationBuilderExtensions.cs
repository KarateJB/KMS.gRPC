using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Kms.Client.Dispatcher.Services;
using Microsoft.AspNetCore.Builder;

namespace Kms.Client.Dispatcher.Utils.Extensions
{
    /// <summary>
    /// IApplicationBuilder extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Register observers for timed services, including audit/renew keys
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        public static void UseKmsClientObservers(this IApplicationBuilder app)
        {
            app.UseTimedAuditKeysObserver();
            app.UseTimedRenewKeysObserver();
        }

        /// <summary>
        /// Configure Auditing keys observer
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        private static void UseTimedAuditKeysObserver(this IApplicationBuilder app)
        {
            var keyDispatcher = app.ApplicationServices.GetService(typeof(IKeyDispatcher)) as IKeyDispatcher;
            var timedAuditKeysService = app.ApplicationServices.GetService(typeof(TimedAuditKeysService)) as TimedAuditKeysService;

            #region Create observable and subscription
            var observable = Observable.FromEventPattern<AuditKeysEventArgs>(
                ev => timedAuditKeysService.AuditKeysEvents += ev,
                ev => timedAuditKeysService.AuditKeysEvents -= ev);

            observable.Subscribe(x =>
            {
                Task.Run(async () =>
                {
                    await keyDispatcher.AuditWorkingKeysAsync();
                    // await keyDispatcher.AuditWorkingKeysBidAsync(); // Bidirection gRPC
                }).Wait();
            });
            #endregion
        }

        /// <summary>
        /// Configure Renew keys observer
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        private static void UseTimedRenewKeysObserver(this IApplicationBuilder app)
        {
            var keyDispatcher = app.ApplicationServices.GetService(typeof(IKeyDispatcher)) as IKeyDispatcher;
            var timedRenewKeysService = app.ApplicationServices.GetService(typeof(TimedRenewKeysService)) as TimedRenewKeysService;

            #region Create observable and subscription
            var observable = Observable.FromEventPattern<RenewKeysEventArgs>(
                ev => timedRenewKeysService.RenewKeysEvents += ev,
                ev => timedRenewKeysService.RenewKeysEvents -= ev);

            observable.Subscribe(x =>
            {
                Task.Run(async () =>
                {
                    await keyDispatcher.RenewKeysBidAsync();
                }).Wait();
            });
            #endregion
        }
    }
}
