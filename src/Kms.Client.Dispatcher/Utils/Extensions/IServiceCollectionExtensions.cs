using System;
using Kms.Client.Dispatcher.Services;
using Kms.Core;
using Kms.Core.Models.Config;
using Kms.KeyMngr.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kms.Client.Dispatcher.Utils.Extensions
{
    /// <summary>
    /// IServiceCollectionExtensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add gRPC clients
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, GrpcOptions grpcOptions)
        {
            services.AddGrpcClient<KeyVaulter.KeyVaulterClient>(o =>
            {
                o.Address = new Uri(grpcOptions.Host);
            }).AddIgnoreValidateCertHttpMessageHandler();

            return services;
        }

        /// <summary>
        /// Add KMS client's required services
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddKmsClient(this IServiceCollection services)
        {
            services.AddKeyManagers();
            services.AddSingleton<IKeyDispatcher, KeyDispatcher>();
            services.AddSingleton<TimedAuditKeysService>();
            services.AddSingleton<TimedRenewKeysService>();

            return services;
        }
    }
}
