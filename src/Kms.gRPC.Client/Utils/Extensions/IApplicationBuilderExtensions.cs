using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Kms.gRPC.Client.Dispatcher;
using Kms.gRPC.Client.Services.Report;
using Kms.gRPC.Client.Services.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kms.gRPC.Client.Utils.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Run startup services
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder RunStartupServices(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var startupServices = serviceProvider.GetServices<IStartupService>();
            if (startupServices != null)
            {
                var sortedServices = startupServices.OrderBy(x => x.Priority);
                foreach (var service in sortedServices)
                {
                    service.StartAsync();  // Set .Wait if you want start the program when all startup services complete
                }
            }

            return app;
        }

        /// <summary>
        /// Configure Auditing key observer
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        public static void UseTimedAuditKeysService(this IApplicationBuilder app)
        {
            var keyDispatcher = app.ApplicationServices.GetService(typeof(IKeyDispatcher)) as IKeyDispatcher;
            var timedClientReportNotifyService = app.ApplicationServices.GetService(typeof(TimedAuditKeyService)) as TimedAuditKeyService;

            #region Create observable and subscription
            var observable = Observable.FromEventPattern<AuditKeyEventArgs>(
                ev => timedClientReportNotifyService.ReportNotify += ev,
                ev => timedClientReportNotifyService.ReportNotify -= ev);

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
        /// Configure TimedKeyCheckService's observer
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        //public static void UseTimedKeyCheckService(this IApplicationBuilder app)
        //{
        //    var keyEventService = app.ApplicationServices.GetService(typeof(KeyEventService)) as KeyEventService;
        //    var timedKeyCheckService = app.ApplicationServices.GetService(typeof(TimedKeyCheckService)) as TimedKeyCheckService;

        //    #region Create observable and subscription
        //    var observable = Observable.FromEventPattern<KeyCheckEventArgs>(
        //        ev => timedKeyCheckService.RenewKey += ev,
        //        ev => timedKeyCheckService.RenewKey -= ev).Select(x => x.EventArgs.DeprecatedKeys);

        //    observable.Subscribe(x =>
        //    {
        //        Task.Run(async () =>
        //        {
        //            var deprecatedKeys = x;

        //            #region Renew Symmetric keys process
        //            var deprecatedSymmetricKeys = deprecatedKeys.Where(k => k.KeyType.Equals(KeyTypeEnum.TripleDES)).ToList().AsReadOnly();
        //            await keyEventService.RenewSymmetricKeyAsync(deprecatedSymmetricKeys);
        //            #endregion

        //            #region Renew Asymmetric keys process
        //            var deprecatedAsymmetricKeys = deprecatedKeys.Where(k => k.KeyType.Equals(KeyTypeEnum.RSA)).ToList().AsReadOnly();
        //            await keyEventService.RenewAsymmetricKeyAsync(deprecatedAsymmetricKeys);
        //            #endregion

        //        }).Wait();
        //    });
        //    #endregion
        //}
    }
}
