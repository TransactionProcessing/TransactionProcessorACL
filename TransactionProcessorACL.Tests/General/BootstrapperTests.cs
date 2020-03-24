using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.Tests.General
{
    using System.Diagnostics;
    using System.Linq;
    using Common;
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
            Startup.Configuration = this.SetupMemoryConfiguration();

            s.ConfigureServices(services);

            this.AddTestRegistrations(services, hostingEnvironment.Object);

            services.AssertConfigurationIsValid();
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
}
