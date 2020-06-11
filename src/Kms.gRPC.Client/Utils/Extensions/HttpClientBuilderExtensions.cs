using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Kms.gRPC.Client.Utils.Extensions
{
    /// <summary>
    /// HttpClientBuilderExtensions
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// AddIgnoreValidateCertHttpMessageHandler
        /// </summary>
        /// <param name="builder">IHttpClientBuilder</param>
        /// <returns>IHttpClientBuilder</returns>
        public static IHttpClientBuilder AddIgnoreValidateCertHttpMessageHandler(this IHttpClientBuilder builder)
        {
            builder.ConfigurePrimaryHttpMessageHandler(h =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });

            return builder;
        }
    }
}
