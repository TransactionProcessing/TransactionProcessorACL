using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TransactionProcessorACL
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using Common;
    using DataTransferObjects;
    using DataTransferObjects.Responses;
    using Factories;
    using HealthChecks.UI.Client;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using NLog.Extensions.Logging;
    using SecurityService.Client;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using Swashbuckle.AspNetCore.Filters;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using TransactionProcessor.Client;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(webHostEnvironment.ContentRootPath)
                                                                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                                      .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true).AddEnvironmentVariables();

            Startup.Configuration = builder.Build();
            Startup.WebHostEnvironment = webHostEnvironment;
        }

        public static IConfigurationRoot Configuration { get; set; }

        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigurationReader.Initialise(Startup.Configuration);

            this.ConfigureMiddlewareServices(services);

            services.AddTransient<IMediator, Mediator>();

            services.AddTransient<ServiceFactory>(context =>
                                                  {
                                                      return t => context.GetService(t);
                                                  });
            services.AddSingleton<IModelFactory, ModelFactory>();
            services.AddSingleton<IRequestHandler<VersionCheckRequest,Unit>, VersionCheckRequestHandler>();
            services.AddSingleton<IRequestHandler<ProcessLogonTransactionRequest, ProcessLogonTransactionResponse>, ProcessLogonTransactionRequestHandler>();
            services.AddSingleton<IRequestHandler<ProcessSaleTransactionRequest, ProcessSaleTransactionResponse>, ProcessSaleTransactionRequestHandler>();
            services.AddSingleton<IRequestHandler<ProcessReconciliationRequest, ProcessReconciliationResponse>, ProcessReconciliationRequestHandler>();
            services.AddSingleton<IRequest<ProcessLogonTransactionResponse>, ProcessLogonTransactionRequest>();
            services.AddSingleton<IRequest<ProcessSaleTransactionResponse>, ProcessSaleTransactionRequest>();
            services.AddSingleton<IRequest<ProcessReconciliationResponse>, ProcessReconciliationRequest>();
            services.AddSingleton<ITransactionProcessorACLApplicationService, TransactionProcessorACLApplicationService>();
            services.AddSingleton<ITransactionProcessorClient, TransactionProcessorClient>();
            services.AddSingleton<ISecurityServiceClient, SecurityServiceClient>();
            services.AddSingleton<Func<String, String>>(container => (serviceName) =>
                                                                     {
                                                                         return ConfigurationReader.GetBaseServerUri(serviceName).OriginalString;
                                                                     });
            HttpClientHandler httpClientHandler = new HttpClientHandler
                                                  {
                                                      ServerCertificateCustomValidationCallback = (message,
                                                                                                   certificate2,
                                                                                                   arg3,
                                                                                                   arg4) =>
                                                                                                  {
                                                                                                      return true;
                                                                                                  }
                                                  };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            services.AddSingleton<HttpClient>(httpClient);
        }

        private void ConfigureMiddlewareServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddUrlGroup(new Uri($"{ConfigurationReader.GetValue("SecurityConfiguration", "Authority")}/health"),
                                 name: "Security Service",
                                 httpMethod: HttpMethod.Get,
                                 failureStatus: HealthStatus.Unhealthy,
                                 tags: new string[] { "security", "authorisation" })
                    .AddUrlGroup(new Uri($"{ConfigurationReader.GetValue("AppSettings", "TransactionProcessorApi")}/health"),
                             name: "Transaction Processor Service",
                             httpMethod: HttpMethod.Get,
                             failureStatus: HealthStatus.Unhealthy,
                             tags: new string[] { "application", "transactionprocessing" });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                                   {
                                       Title = "Transaction Processor ACL",
                                       Version = "1.0",
                                       Description = "A REST Api to provide and Anti Corruption Layer for the Transaction Mobile Application",
                                       Contact = new OpenApiContact
                                                 {
                                                     Name = "Stuart Ferguson",
                                                     Email = "golfhandicapping@btinternet.com"
                                                 }
                                   });
                c.UseAllOfForInheritance();
                c.SelectSubTypesUsing(baseType =>
                                      {
                                          return typeof(TransactionRequestMessage).Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType));
                                      });

                c.SelectSubTypesUsing(baseType =>
                                      {
                                          return typeof(TransactionResponseMessage).Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType));
                                      });
                // add a custom operation filter which sets default values
                c.OperationFilter<SwaggerDefaultValues>();
                c.ExampleFilters();

                //Locate the XML files being generated by ASP.NET...
                var directory = new DirectoryInfo(AppContext.BaseDirectory);
                var xmlFiles = directory.GetFiles("*.xml");

                //... and tell Swagger to use those XML comments.
                foreach (FileInfo fileInfo in xmlFiles)
                {
                    c.IncludeXmlComments(fileInfo.FullName);
                }
            });

            services.AddSwaggerExamplesFromAssemblyOf<SwaggerJsonConverter>();

            services.AddAuthentication(options =>
                                       {
                                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                           options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                       })
                    .AddJwtBearer(options =>
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                                                         {
                                                             ServerCertificateCustomValidationCallback =
                                                                 (message, certificate, chain, sslPolicyErrors) => true
                                                         };
                        options.Authority = ConfigurationReader.GetValue("SecurityConfiguration", "Authority");
                        options.Audience = ConfigurationReader.GetValue("SecurityConfiguration", "ApiName");

                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                                                            {
                                                                ValidateAudience = false,
                                                                ValidAudience = ConfigurationReader.GetValue("SecurityConfiguration", "ApiName"),
                                                                ValidIssuer = ConfigurationReader.GetValue("SecurityConfiguration", "Authority"),
                                                            };
                        options.IncludeErrorDetails = true;
                    });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            services.AddMvcCore().AddApplicationPart(assembly).AddControllersAsServices();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            String nlogConfigFilename = "nlog.config";

            if (env.IsDevelopment())
            {
                nlogConfigFilename = $"nlog.{env.EnvironmentName}.config";
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.ConfigureNLog(Path.Combine(env.ContentRootPath, nlogConfigFilename));
            loggerFactory.AddNLog();

            ILogger logger = loggerFactory.CreateLogger("TransactionProcessor");

            Logger.Initialise(logger);

            app.AddRequestLogging();
            app.AddResponseLogging();
            app.AddExceptionHandler();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapControllers();
                                 endpoints.MapHealthChecks("health", new HealthCheckOptions()
                                                                     {
                                                                         Predicate = _ => true,
                                                                         ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                                                                     });
                             });

            app.UseSwagger();

            app.UseSwaggerUI();
        }
    }
}
