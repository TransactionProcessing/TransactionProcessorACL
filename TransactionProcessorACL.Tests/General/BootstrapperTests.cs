using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.Tests.General
{
    using System.Diagnostics;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Shared.General;
    using Xunit;

    public class BootstrapperTests
    {
        #region Methods

        /// <summary>
        /// Verifies the bootstrapper is valid.
        /// </summary>
        [Fact]
        public void VerifyBootstrapperIsValid()
        {
            Mock<IWebHostEnvironment> hostingEnvironment = new Mock<IWebHostEnvironment>();
            hostingEnvironment.Setup(he => he.EnvironmentName).Returns("Development");
            hostingEnvironment.Setup(he => he.ContentRootPath).Returns("/home");
            hostingEnvironment.Setup(he => he.ApplicationName).Returns("Test Application");

            IServiceCollection services = new ServiceCollection();
            Startup s = new Startup(hostingEnvironment.Object);
            s.ConfigureServices(services);

            Startup.Configuration = this.SetupMemoryConfiguration();
            ConfigurationReader.Initialise(Startup.Configuration);

            this.AddTestRegistrations(services, hostingEnvironment.Object);

            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);

            s.ConfigureContainer(builder);

            IContainer container = builder.Build();

            using(ILifetimeScope scope = container.BeginLifetimeScope())
            {
                scope.ResolveAll(new List<String>());
            }
        }

        private IConfigurationRoot SetupMemoryConfiguration()
        {
            Dictionary<String, String> configuration = new Dictionary<String, String>();

            IConfigurationBuilder builder = new ConfigurationBuilder();

            configuration.Add("AppSettings:SecurityService", "http://192.168.1.133:5001");
            configuration.Add("AppSettings:TransactionProcessorApi", "http://192.168.1.133:5002");

            builder.AddInMemoryCollection(configuration);

            return builder.Build();
        }

        /// <summary>
        /// Adds the test registrations.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        private void AddTestRegistrations(IServiceCollection services,
                                          IWebHostEnvironment hostingEnvironment)
        {
            services.AddLogging();
            DiagnosticListener diagnosticSource = new DiagnosticListener(hostingEnvironment.ApplicationName);
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddSingleton(diagnosticSource);
            services.AddSingleton(hostingEnvironment);
        }

        #endregion
    }

    public static class ScopeExtensions
    {
        #region Methods

        /// <summary>
        /// Filters the specified ignored assemblies.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="ignoredAssemblies">The ignored assemblies.</param>
        /// <returns></returns>
        public static IList<IServiceWithType> Filter(this IEnumerable<IServiceWithType> services,
                                                     IEnumerable<String> ignoredAssemblies)
        {
            return services.Where(serviceWithType => ignoredAssemblies.All(ignored => ignored != serviceWithType.ServiceType.FullName)).ToList();
        }

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ignoredAssemblies">The ignored assemblies.</param>
        /// <returns></returns>
        public static IList<Object> ResolveAll(this ILifetimeScope scope,
                                               IEnumerable<String> ignoredAssemblies)
        {
            var services = scope.ComponentRegistry.Registrations.SelectMany(x => x.Services).OfType<IServiceWithType>().Filter(ignoredAssemblies).ToList();

            foreach (var serviceWithType in services)
            {
                try
                {
                    scope.Resolve(serviceWithType.ServiceType);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return services.Select(x => x.ServiceType).Select(scope.Resolve).ToList();
        }

        #endregion
    }
}
