using Kms.Client.Dispatcher.Services;
using Kms.Client.Dispatcher.Utils.Extensions;
using Kms.Core.Models.Config.Client;
using Kms.gRPC.Client.Services.Report;
using Kms.gRPC.Client.Services.Startup;
using Kms.gRPC.Client.Utils.Extensions;
using Kms.KeyMngr.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kms.gRPC.Client
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Inject AppSettings configuration
            services.Configure<AppSettings>(this.Configuration);
            #endregion

            #region gRPC client factory
            services.AddGrpcClients(this.appSettings.Grpc);
            #endregion

            #region Custom services
            services.AddKmsClient();
            services.AddSingleton<IKeyDispatcher, KeyDispatcher>();
            services.AddSingleton<TimedAuditKeyService>();
            // services.AddSingleton<TimedKeyCheckService>();
            #endregion

            #region Startup services
            // Startup service to initialize key(s)
            services.AddSingleton<IStartupService, SyncKeysStartupService>();
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
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                ////app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            #region Register audit key observer
            app.UseTimedAuditKeysService();
            #endregion

            #region Run startup services
            app.RunStartupServices();
            #endregion
        }
    }
}
