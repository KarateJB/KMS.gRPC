using Kms.Core.Models.Config.Server;
using Kms.gRPC.Services.gRPC;
using Kms.gRPC.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kms.gRPC
{
    public class Startup
    {
        private readonly IWebHostEnvironment env = null;
        private readonly AppSettings appSettings = null;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.env = env;
            this.appSettings = new AppSettings();
            this.Configuration.Bind(this.appSettings);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Inject AppSettings configuration
            services.Configure<AppSettings>(this.Configuration);
            #endregion

            #region MemoryCache
            services.AddMemoryCache();
            #endregion

            #region gRPC
            services.AddGrpc();
            #endregion

            #region Custom services
            services.AddCustomServices();
            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreetService>();
                endpoints.MapGrpcService<KeyVaultService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
