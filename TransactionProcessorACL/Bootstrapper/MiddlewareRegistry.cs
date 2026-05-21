using Microsoft.OpenApi;
using Shared.Authorisation;

namespace TransactionProcessorACL.Bootstrapper
{
    using DataTransferObjects;
    using DataTransferObjects.Responses;
    using Lamar;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using OpenIddict.Validation.AspNetCore;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Serialisation;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Json;

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
            return new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback = (message,
                                                                    cert,
                                                                    chain,
                                                                    errors) =>
                                                                   {
                                                                       return true;
                                                                   }
                   };
        }

        private void ConfigureAuthentication()
        {
            this.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            this.AddOpenIddict()
                .AddValidation(options =>
                {
                    // Same as your Authority
                    options.SetIssuer(new Uri(ConfigurationReader.GetValue("SecurityConfiguration", "Authority")));

                    // Enables discovery and HTTP backchannel support
                    options.UseSystemNetHttp()
                        .ConfigureHttpClientHandler(handler =>
                        {
                            // DEV ONLY: bypass all certificate errors
                            handler.ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        });

                    // Register the ASP.NET Core integration
                    options.UseAspNetCore();

                    // Optionally set expected audience(s):
                    options.AddAudiences(ConfigurationReader.GetValue("SecurityConfiguration", "ApiName"));



                });

            this.AddAuthorization();
        }

        private void ConfigureControllers()
        {
            this.AddControllers();
            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            this.AddMvcCore().AddApplicationPart(assembly).AddControllersAsServices();
        }

        private void ConfigureHealthChecks()
        {
            this.AddHealthChecks().AddSecurityService(this.ApiEndpointHttpHandler).AddTransactionProcessorService();
        }

        private void ConfigureJsonOptions()
        {
            this.ConfigureHttpJsonOptions(options => {
                JsonSerializerConfiguration.ConfigureMinimalApi(options.SerializerOptions);
            });
        }

        private void ConfigurePasswordTokenHandling()
        {
            this.AddPasswordTokenPolicy();
            this.AddPasswordTokenHandler();
        }
    }

    public static class JsonSerializerConfiguration
    {
        public static void ConfigureMinimalApi(JsonSerializerOptions serializerOptions)
        {
            var defaultOptions = SystemTextJsonSerializer.GetDefaultJsonSerializerOptions();
            serializerOptions.PropertyNamingPolicy = defaultOptions.PropertyNamingPolicy;
            serializerOptions.DictionaryKeyPolicy = defaultOptions.DictionaryKeyPolicy;
            serializerOptions.ReferenceHandler = defaultOptions.ReferenceHandler;
            serializerOptions.WriteIndented = defaultOptions.WriteIndented;
            serializerOptions.Converters.Add(new DateTimeSpaceConverter());
        }
    }
}
