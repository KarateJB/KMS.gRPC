using System.Linq;
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
    }
}
