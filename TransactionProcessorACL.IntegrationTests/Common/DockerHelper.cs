﻿namespace TransactionProcessor.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Common;
    using Ductus.FluentDocker.Model.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using EstateManagement.Client;
    using EstateReporting.Database;
    using EventStore.Client;
    using global::Shared.IntegrationTesting;
    using global::Shared.Logger;
    using Microsoft.Data.SqlClient;
    using SecurityService.Client;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.IntegrationTesting.DockerHelper" />
    public class DockerHelper : global::Shared.IntegrationTesting.DockerHelper
    {
        #region Fields

        /// <summary>
        /// The estate client
        /// </summary>
        public IEstateClient EstateClient;

        /// <summary>
        /// The HTTP client
        /// </summary>
        public HttpClient HttpClient;

        /// <summary>
        /// The security service client
        /// </summary>
        public ISecurityServiceClient SecurityServiceClient;

        /// <summary>
        /// The transaction processor client
        /// </summary>
        public ITransactionProcessorClient TransactionProcessorClient;

        private const String MinimumSupportedApplicationVersion = "1.0.5";

        private readonly TestingContext TestingContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DockerHelper()
        {
            this.TestingContext = new TestingContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName)
        {
            await base.StartContainersForScenarioRun(scenarioName);

            
            // Setup the base address resolvers
            String EstateManagementBaseAddressResolver(String api) => $"http://127.0.0.1:{this.EstateManagementPort}";
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String TransactionProcessorBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TransactionProcessorPort}";
            String TransactionProcessorAclBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TransactionProcessorAclPort}";

            HttpClientHandler clientHandler = new HttpClientHandler
                                              {
                                                  ServerCertificateCustomValidationCallback = (message,
                                                                                               certificate2,
                                                                                               arg3,
                                                                                               arg4) =>
                                                                                              {
                                                                                                  return true;
                                                                                              }

                                              };
            HttpClient httpClient = new HttpClient(clientHandler);
            this.EstateClient = new EstateClient(EstateManagementBaseAddressResolver, httpClient);
            this.SecurityServiceClient = new SecurityServiceClient(SecurityServiceBaseAddressResolver, httpClient);
            this.TransactionProcessorClient = new TransactionProcessorClient(TransactionProcessorBaseAddressResolver, httpClient);

            this.HttpClient = new HttpClient();
            this.HttpClient.BaseAddress = new Uri(TransactionProcessorAclBaseAddressResolver(string.Empty));

        }
        
       private async Task RemoveEstateReadModel()
        {
            List<Guid> estateIdList = this.TestingContext.GetAllEstateIds();

            foreach (Guid estateId in estateIdList)
            {
                String databaseName = $"EstateReportingReadModel{estateId}";

                // Build the connection string (to master)
                String connectionString = Setup.GetLocalConnectionString(databaseName);
                await Retry.For(async () =>
                                {
                                    EstateReportingSqlServerContext context = new EstateReportingSqlServerContext(connectionString);
                                    await context.Database.EnsureDeletedAsync(CancellationToken.None);
                                }, retryFor: TimeSpan.FromMinutes(2), retryInterval: TimeSpan.FromSeconds(30));
            }
        }

        /// <summary>
        /// Stops the containers for scenario run.
        /// </summary>
        public override async Task StopContainersForScenarioRun()
        {
            await RemoveEstateReadModel().ConfigureAwait(false);

            base.StopContainersForScenarioRun();
        }

        public override async Task<IContainerService> SetupSecurityServiceContainer(List<INetworkService> networkServices,
                                                                                    List<String> additionalEnvironmentVariables = null)
        {
            this.Trace("About to Start Security Container");

            List<String> environmentVariables = this.GetCommonEnvironmentVariables(DockerPorts.SecurityServiceDockerPort);
            environmentVariables.Add($"ServiceOptions:PublicOrigin=https://{this.SecurityServiceContainerName}:{DockerPorts.SecurityServiceDockerPort}");
            environmentVariables.Add($"ServiceOptions:IssuerUrl=https://{this.SecurityServiceContainerName}:{DockerPorts.SecurityServiceDockerPort}");
            environmentVariables.Add("ASPNETCORE_ENVIRONMENT=IntegrationTest");
            environmentVariables.Add($"urls=https://*:{DockerPorts.SecurityServiceDockerPort}");

            environmentVariables.Add($"ServiceOptions:PasswordOptions:RequiredLength=6");
            environmentVariables.Add($"ServiceOptions:PasswordOptions:RequireDigit=false");
            environmentVariables.Add($"ServiceOptions:PasswordOptions:RequireUpperCase=false");
            environmentVariables.Add($"ServiceOptions:UserOptions:RequireUniqueEmail=false");
            environmentVariables.Add($"ServiceOptions:SignInOptions:RequireConfirmedEmail=false");

            if (additionalEnvironmentVariables != null)
            {
                environmentVariables.AddRange(additionalEnvironmentVariables);
            }

            ContainerBuilder securityServiceContainer = new Builder().UseContainer().WithName(this.SecurityServiceContainerName)
                                                                     .WithEnvironment(environmentVariables.ToArray())
                                                                     .UseImageDetails(this.GetImageDetails(ContainerType.SecurityService))
                                                                     .ExposePort(DockerPorts.SecurityServiceDockerPort)
                                                                     .MountHostFolder(this.HostTraceFolder)
                                                                     .SetDockerCredentials(this.DockerCredentials);

            // Now build and return the container                
            IContainerService builtContainer = securityServiceContainer.Build().Start().WaitForPort($"{DockerPorts.SecurityServiceDockerPort}/tcp", 30000);

            foreach (INetworkService networkService in networkServices)
            {
                networkService.Attach(builtContainer, false);
            }

            this.Trace("Security Service Container Started");
            this.Containers.Add(builtContainer);

            //  Do a health check here
            this.SecurityServicePort = builtContainer.ToHostExposedEndpoint($"{DockerPorts.SecurityServiceDockerPort}/tcp").Port;
            await this.DoHealthCheck(ContainerType.SecurityService);

            return builtContainer;
        }

        #endregion
    }
}