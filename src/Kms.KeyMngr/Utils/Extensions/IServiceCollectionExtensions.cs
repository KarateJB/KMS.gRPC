using System.Linq;
using Kms.KeyMngr.KeyManager;
using Microsoft.Extensions.DependencyInjection;

namespace Kms.KeyMngr.Utils.Extensions
{
    /// <summary>
    /// Delegate for resolve IKeyManager
    /// </summary>
    /// <param name="className">Class name</param>
    /// <returns>The implemenetation of IKeyManager</returns>
    public delegate IKeyManager KeyManagerResolver(string className);

    /// <summary>
    /// ServiceCollectionExtensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add KmsClient services
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddKeyManagers(this IServiceCollection services)
        {
            #region MemoryCache
            services.AddMemoryCache();
            #endregion

            #region Add Key management services
            services.AddSingleton<IKeyManager, TripleDesKeyManager>();
            services.AddSingleton<IKeyManager, SharedSecretKeyManager>();
            services.AddSingleton<IKeyManager, RsaKeyManager>();

            services.AddSingleton<KeyManagerResolver>(sp => className =>
            {
                var keyManager = sp.GetServices<IKeyManager>().Where(x => x.GetType().Name.Equals(className)).FirstOrDefault();
                return keyManager;
            });
            #endregion

            return services;
        }
    }
}
