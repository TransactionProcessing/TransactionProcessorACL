using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TransactionProcessorACL.Endpoints;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TransactionProcessorACL
{
    using Bootstrapper;
    using HealthChecks.UI.Client;
    using Lamar;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using Shared.Middleware;
    using Shared.Serialisation;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using TransactionProcessorACL.Common;
    using TransactionProcessorACL.Middleware;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment webHostEnvironment)
        {
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
            services.IncludeRegistry<SerialiserRegistry>();

            Startup.Container = new Container(services);

            var serialiser = Container.GetRequiredService<IStringSerialiser>();
            StringSerialiser.Initialise(serialiser);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
                              IHostApplicationLifetime lifetime, IHost host)
        {
            UseDevelopmentExceptionPage(app, env);
            InitializeLogger(loggerFactory);
            ConfigureMiddleware(app);
            ConfigureEndpoints(app);
        }

        private static void UseDevelopmentExceptionPage(IApplicationBuilder app,
                                                        IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private static void InitializeLogger(ILoggerFactory loggerFactory)
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
            app.UseRouting();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/api/reporting") &&
                    context.User.Identity?.IsAuthenticated == true)
                {
                    ILogger logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("TransactionProcessorACL.AuthenticationDiagnostics");

                    String claims = String.Join("; ", context.User.Claims.Select(claim => $"{claim.Type}={claim.Value}"));
                    String authorizationMetadata = String.Join(", ", context.GetEndpoint()?.Metadata
                        .GetOrderedMetadata<IAuthorizeData>()
                        .Select(metadata => $"Policy={metadata.Policy ?? "<default>"}; Roles={metadata.Roles ?? "<none>"}")
                        ?? Enumerable.Empty<String>());
                    logger.LogWarning("Temporary authentication diagnostics: AuthenticationType={AuthenticationType}; Claims={Claims}",
                                     context.User.Identity.AuthenticationType,
                                     claims);
                    logger.LogWarning("Temporary endpoint authorization diagnostics: Endpoint={Endpoint}; Metadata={Metadata}",
                                     context.GetEndpoint()?.DisplayName,
                                     authorizationMetadata);
                }

                await next();
            });
            app.UseAuthorization();
            app.UseMiddleware<RequestAuditMiddleware>();
            app.UseMiddleware<VersionCheckMiddleware>();
        }

        private static void ConfigureEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(MapEndpoints);
        }

        private static void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapMerchantEndpoints();
            endpoints.MapReportingEndpoints();
            endpoints.MapTransactionEndpoints();
            endpoints.MapVoucherEndpoints();
            endpoints.MapHealthChecks("health", CreateHealthCheckOptions(Shared.HealthChecks.HealthCheckMiddleware.WriteResponse));
            endpoints.MapHealthChecks("healthui", CreateHealthCheckOptions(UIResponseWriter.WriteHealthCheckUIResponse));
        }

        private static HealthCheckOptions CreateHealthCheckOptions(Func<HttpContext, HealthReport, Task> responseWriter)
        {
            return new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = responseWriter
            };
        }
    }
}
