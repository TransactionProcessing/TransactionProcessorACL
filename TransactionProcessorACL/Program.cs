using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Shared.General;
using Shared.Extensions;
using Shared.Logger;
using Shared.Middleware;
using Shared.Serialisation;
using Sentry.Extensibility;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using TransactionProcessorACL.Bootstrapper;
using TransactionProcessorACL.Endpoints;
using TransactionProcessorACL.Middleware;
using HealthChecks.UI.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionProcessing.ApiDefaults;
using TransactionProcessorACL.Common;

namespace TransactionProcessorACL;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        WebApplication app = CreateWebApplication(args);
        app.Run();
    }

    public static WebApplication CreateWebApplication(string[] args)
    {
        string contentRoot = Directory.GetCurrentDirectory();
        string nlogConfigPath = Path.Combine(contentRoot, "nlog.config");

        LogManager.Setup(builder =>
        {
            builder.SetupLogFactory(setup =>
            {
                setup.AddCallSiteHiddenAssembly(typeof(NlogLogger).Assembly);
                setup.AddCallSiteHiddenAssembly(typeof(Shared.Logger.Logger).Assembly);
                setup.AddCallSiteHiddenAssembly(typeof(TenantMiddleware).Assembly);
            });
            builder.LoadConfigurationFromFile(nlogConfigPath);
        });

        WebApplicationBuilder appBuilder = WebApplication.CreateBuilder(args);
        appBuilder.Host.UseWindowsService();
        appBuilder.Host.UseLamar();
        appBuilder.Logging.AddConsole();
        appBuilder.Logging.AddNLog();
        appBuilder.WebHost.UseKestrel();

        ConfigureApplicationConfiguration(appBuilder);

        IConfigurationRoot configuration = appBuilder.Configuration;
        ConfigurationReader.Initialise(configuration);
        ConfigureServices(appBuilder, configuration);
        ConfigureSentry(appBuilder, configuration);

        WebApplication app = appBuilder.Build();
        StringSerialiser.Initialise(app.Services.GetRequiredService<IStringSerialiser>());
        ConfigurePipeline(app, configuration);

        return app;
    }

    private static void ConfigureApplicationConfiguration(WebApplicationBuilder appBuilder)
    {
        FileInfo assemblyFile = new(Assembly.GetExecutingAssembly().Location);

        appBuilder.Configuration
            .SetBasePath(assemblyFile.Directory!.FullName)
            .AddJsonFile("hosting.json", optional: true)
            .AddJsonFile($"hosting.{appBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddJsonFile("/home/txnproc/config/appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"/home/txnproc/config/appsettings.{appBuilder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{appBuilder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("/home/txnproc/config/appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["HealthMonitoring:Service:Version"] = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0.0"
            });
    }

    private static void ConfigureServices(WebApplicationBuilder appBuilder, IConfiguration configuration)
    {
        appBuilder.Host.ConfigureContainer<ServiceRegistry>((_, services) =>
        {
            services.IncludeRegistry(new MiddlewareRegistry(configuration));
            services.IncludeRegistry<ApplicationServiceRegistry>();
            services.IncludeRegistry<ClientRegistry>();
            services.IncludeRegistry(new MediatorRegistry(configuration));
            services.IncludeRegistry<MiscRegistry>();
            services.IncludeRegistry<SerialiserRegistry>();
        });
    }

    private static void ConfigureSentry(WebApplicationBuilder appBuilder, IConfiguration configuration)
    {
        IConfigurationSection sentrySection = configuration.GetSection("SentryConfiguration");
        if (sentrySection.Exists() && appBuilder.Environment.IsDevelopment() == false)
        {
            appBuilder.WebHost.UseSentry(options =>
            {
                options.Dsn = configuration["SentryConfiguration:Dsn"];
                options.SendDefaultPii = true;
                options.MaxRequestBodySize = RequestSize.Always;
                options.CaptureBlockingCalls = ConfigurationReader.GetValueOrDefault("SentryConfiguration", "CaptureBlockingCalls", false);
                options.IncludeActivityData = ConfigurationReader.GetValueOrDefault("SentryConfiguration", "IncludeActivityData", false);
                options.Release = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            });
        }
    }

    private static void ConfigurePipeline(WebApplication app, IConfigurationRoot configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        LogManager.Setup().LoadConfigurationFromFile("NLog.config");
        NLog.Logger logger = LogManager.GetLogger("TransactionProcessorACL");
        Shared.Logger.Logger.Initialise(logger, "TransactionProcessorACL");
        configuration.LogConfiguration(Shared.Logger.Logger.LogWarning);

        app.UseMiddleware<TenantMiddleware>();
        app.AddRequestResponseLogging();
        app.AddExceptionHandler();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<RequestAuditMiddleware>();
        app.UseMiddleware<VersionCheckMiddleware>();

        MapEndpoints(app);
        app.UseApiDefaults();
    }

    private static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapMerchantEndpoints();
        endpoints.MapReportingEndpoints();
        endpoints.MapTransactionEndpoints();
        endpoints.MapVoucherEndpoints();
        endpoints.MapHealthChecks("health", CreateHealthCheckOptions(Shared.HealthChecks.HealthCheckMiddleware.WriteResponse))
            .WithName("Health")
            .WithTags("Health")
            .WithSummary("Check service health")
            .WithDescription("Runs the configured service health checks and returns their aggregate status. This route does not require an applicationVersion.")
            .WithMetadata(new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(object), new[] { EndpointDocumentationConstants.JsonMediaType }))
            .WithMetadata(new ProducesResponseTypeMetadata(StatusCodes.Status503ServiceUnavailable, typeof(object), new[] { EndpointDocumentationConstants.JsonMediaType }));
        endpoints.MapHealthChecks("healthui", CreateHealthCheckOptions(UIResponseWriter.WriteHealthCheckUIResponse))
            .WithName("HealthUi")
            .WithTags("Health")
            .WithSummary("Get health check details")
            .WithDescription("Runs all configured service health checks and returns the detailed Health Checks UI response. This route does not require an applicationVersion.")
            .WithMetadata(new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(object), new[] { EndpointDocumentationConstants.JsonMediaType }))
            .WithMetadata(new ProducesResponseTypeMetadata(StatusCodes.Status503ServiceUnavailable, typeof(object), new[] { EndpointDocumentationConstants.JsonMediaType }));
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
