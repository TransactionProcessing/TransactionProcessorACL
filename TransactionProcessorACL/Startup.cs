using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleResults;
using TransactionProcessorACL.Endpoints;

namespace TransactionProcessorACL
{
    using Bootstrapper;
    using BusinessLogic.Requests;
    using HealthChecks.UI.Client;
    using Lamar;
    using MediatR;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using Shared.Middleware;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using TransactionProcessorACL.Middleware;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(webHostEnvironment.ContentRootPath)
                                                                      .AddJsonFile("/home/txnproc/config/appsettings.json", true, true)
                                                                      .AddJsonFile($"/home/txnproc/config/appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true)
                                                                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                                      .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                                                      .AddEnvironmentVariables();

            Startup.Configuration = builder.Build();
            Startup.WebHostEnvironment = webHostEnvironment;
        }

        public static IConfigurationRoot Configuration { get; set; }

        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public static Container Container;

        public void ConfigureContainer(ServiceRegistry services)
        {
            ConfigurationReader.Initialise(Startup.Configuration);

            services.IncludeRegistry<MiddlewareRegistry>();
            services.IncludeRegistry<ApplicationServiceRegistry>();
            services.IncludeRegistry<ClientRegistry>();
            services.IncludeRegistry<MediatorRegistry>();
            services.IncludeRegistry<MiscRegistry>();

            Startup.Container = new Container(services);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            Startup.UseDevelopmentExceptionPage(app, env);
            Startup.InitialiseLogger(loggerFactory);
            Startup.ConfigureMiddleware(app);
            Startup.ConfigureEndpoints(app);
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        private static void UseDevelopmentExceptionPage(IApplicationBuilder app,
                                                        IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private static void InitialiseLogger(ILoggerFactory loggerFactory)
        {
            ILogger logger = loggerFactory.CreateLogger("TransactionProcessor");

            Logger.Initialise(logger);
            Startup.Configuration.LogConfiguration(Logger.LogWarning);
        }

        private static void ConfigureMiddleware(IApplicationBuilder app)
        {
            app.UseMiddleware<TenantMiddleware>();
            app.AddRequestResponseLogging();
            app.AddExceptionHandler();
            app.UseMiddleware<VersionCheckMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(Startup.MapEndpoints);
        }

        private static void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapMerchantEndpoints();
            endpoints.MapTransactionEndpoints();
            endpoints.MapVoucherEndpoints();
            endpoints.MapHealthChecks("health", Startup.CreateHealthCheckOptions(Shared.HealthChecks.HealthCheckMiddleware.WriteResponse));
            endpoints.MapHealthChecks("healthui", Startup.CreateHealthCheckOptions(UIResponseWriter.WriteHealthCheckUIResponse));
        }

        private static HealthCheckOptions CreateHealthCheckOptions(Func<HttpContext, HealthReport, CancellationToken, Task> responseWriter)
        {
            return new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = responseWriter
            };
        }
    }
}
