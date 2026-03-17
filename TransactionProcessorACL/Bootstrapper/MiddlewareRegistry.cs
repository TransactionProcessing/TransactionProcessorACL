using Microsoft.OpenApi;
using Shared.Authorisation;

namespace TransactionProcessorACL.Bootstrapper
{
    using Common;
    using DataTransferObjects;
    using DataTransferObjects.Responses;
    using Lamar;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.Extensions;
    using Shared.General;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Lamar.ServiceRegistry" />
    [ExcludeFromCodeCoverage]
    public class MiddlewareRegistry : ServiceRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiddlewareRegistry"/> class.
        /// </summary>
        public MiddlewareRegistry()
        {
            this.ConfigureHealthChecks();
            this.ConfigureSwagger();
            this.ConfigureAuthentication();
            this.ConfigurePasswordTokenHandling();
            this.ConfigureJsonOptions();
            this.ConfigureControllers();
        }

        /// <summary>
        /// APIs the endpoint HTTP handler.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        private HttpClientHandler ApiEndpointHttpHandler(IServiceProvider serviceProvider)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            if (this.AllowInvalidServerCertificates(serviceProvider))
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message,
                                                                               cert,
                                                                               chain,
                                                                               errors) =>
                                                                              {
                                                                                  return true;
                                                                              };
            }

            return httpClientHandler;
        }

        private void ConfigureAuthentication()
        {
            this.AddAuthentication(options =>
                                   {
                                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                   })
                .AddJwtBearer(options =>
                              {
                                  if (this.AllowInvalidServerCertificates())
                                  {
                                      options.BackchannelHttpHandler = new HttpClientHandler
                                                                       {
                                                                           ServerCertificateCustomValidationCallback =
                                                                               (message, certificate, chain, sslPolicyErrors) => true
                                                                       };
                                  }

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
        }

        private void ConfigureControllers()
        {
            this.AddControllers();
            //.AddNewtonsoftJson(options =>
            //                                        {
            //                                            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //                                            options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            //                                            options.SerializerSettings.Formatting = Formatting.Indented;
            //                                            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            //                                            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //                                        });

            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            this.AddMvcCore().AddApplicationPart(assembly).AddControllersAsServices();
        }

        private void ConfigureHealthChecks()
        {
            this.AddHealthChecks().AddSecurityService(this.ApiEndpointHttpHandler).AddTransactionProcessorService();
        }

        private void ConfigureJsonOptions()
        {
            this.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                options.SerializerOptions.PropertyNameCaseInsensitive = true; // optional, but safer
            });
        }

        private void ConfigurePasswordTokenHandling()
        {
            this.AddPasswordTokenPolicy();
            this.AddPasswordTokenHandler();
        }

        private void ConfigureSwagger()
        {
            this.AddSwaggerGen(c =>
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
            this.AddSwaggerExamplesFromAssemblyOf<SwaggerJsonConverter>();
        }

        private Boolean AllowInvalidServerCertificates(IServiceProvider serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                var webHostEnvironment = serviceProvider.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();

                if (webHostEnvironment != null)
                {
                    return String.Equals(webHostEnvironment.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase);
                }
            }

            return Startup.WebHostEnvironment != null &&
                   String.Equals(Startup.WebHostEnvironment.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase);
        }
    }
}
