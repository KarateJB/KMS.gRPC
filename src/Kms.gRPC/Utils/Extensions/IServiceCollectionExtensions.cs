using Kms.gRPC.Services.Cache;
using Kms.gRPC.Services.DataProtection;
using Kms.gRPC.Services.Report;
using Microsoft.Extensions.DependencyInjection;

namespace Kms.gRPC.Utils.Extensions
{
    /// <summary>
    /// IServiceCollection extensions
    /// </summary>

    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add custom services
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<RedisService>();
            services.AddSingleton<IKeyVault, KeyVault>();
            services.AddSingleton<IKeyAuditReporter, KeyAuditReporter>();
            
            return services;
        }
    }
}
