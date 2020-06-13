using System;
using Kms.gRPC.Client.Services.Startup;
using Kms.Core;
using Kms.Core.Models.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Kms.gRPC.Client.Utils.Extensions
{
    /// <summary>
    /// IServiceCollectionExtensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add startup services
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddStartupServices(this IServiceCollection services)
        {
            services.AddSingleton<IStartupService, SyncKeysStartupService>();
            return services;
        }
    }
}
