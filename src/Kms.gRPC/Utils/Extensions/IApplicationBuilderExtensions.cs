using System;
using Kms.Core.Models.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kms.gRPC.Utils.Extensions
{
    /// <summary>
    /// ApplicationBuilderExtensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// ConfigureExceptionHandler
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Global Exception Handler");
            app.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    // Get exception
                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = feature?.Error ?? new Exception("Internal Server Error");

                    // Logging
                    if (exception.GetType().Equals(typeof(ApiException)))
                    {
                        var apiException = exception as ApiException;
                        logger.LogError(apiException.Exception, apiException.Message);
                    }
                    else
                    {
                        logger.LogError(exception, exception.Message);
                    }

                    // Custom response
                    context.Response.ContentType = "application/json";
                    var json = $"{{\"error\":\"{exception?.Message}\"}}";
                    await context.Response.WriteAsync(json);
                    return;
                });
            });
        }
    }
}
