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
        /// The test identifier
        /// </summary>
        public Guid TestId;

        /// <summary>
        /// The transaction processor client
        /// </summary>
        public ITransactionProcessorClient TransactionProcessorClient;

        /// <summary>
        /// The containers
        /// </summary>
        protected List<IContainerService> Containers;

        /// <summary>
        /// The estate management API port
        /// </summary>
        protected Int32 EstateManagementApiPort;

        /// <summary>
        /// The event store HTTP port
        /// </summary>
        protected Int32 EventStoreHttpPort;

        /// <summary>
        /// The security service port
        /// </summary>
        protected Int32 SecurityServicePort;

        /// <summary>
        /// The test networks
        /// </summary>
        protected List<INetworkService> TestNetworks;

        /// <summary>
        /// The transaction processor acl port
        /// </summary>
        protected Int32 TransactionProcessorACLPort;
        
        private const String MinimumSupportedApplicationVersion = "1.0.5";
        
        /// <summary>
        /// The transaction processor port
        /// </summary>
        protected Int32 TransactionProcessorPort;

        private readonly TestingContext TestingContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DockerHelper(NlogLogger logger, TestingContext testingContext)
        {
            this.Logger = logger;
            this.TestingContext = testingContext;
            this.Containers = new List<IContainerService>();
            this.TestNetworks = new List<INetworkService>();
        }

        #endregion

        #region Methods

        protected override String GenerateEventStoreConnectionString()
        {
            // TODO: this could move to shared
            String eventStoreAddress = $"esdb://admin:changeit@{this.EventStoreContainerName}:{DockerHelper.EventStoreHttpDockerPort}";
            if (this.IsSecureEventStore)
            {
                eventStoreAddress = $"{eventStoreAddress}?tls=true&tlsVerifyCert=false";
            }
            else
            {
                eventStoreAddress = $"{eventStoreAddress}?tls=false&tlsVerifyCert=false";
            }

            return eventStoreAddress;
        }

        public Boolean IsSecureEventStore { get; private set; }

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName)
        {
            String IsSecureEventStoreEnvVar = Environment.GetEnvironmentVariable("IsSecureEventStore");

            if (IsSecureEventStoreEnvVar == null)
            {
                // No env var set so default to insecure
                this.IsSecureEventStore = false;
            }
            else
            {
                // We have the env var so we set the secure flag based on the value in the env var
                Boolean.TryParse(IsSecureEventStoreEnvVar, out Boolean isSecure);
                this.IsSecureEventStore = isSecure;
            }

            this.HostTraceFolder = FdOs.IsWindows() ? $"D:\\home\\txnproc\\trace\\{scenarioName}" : $"//home//txnproc//trace//{scenarioName}";

            Logging.Enabled();

            Guid testGuid = Guid.NewGuid();
            this.TestId = testGuid;

            this.Logger.LogInformation($"Test Id is {testGuid}");

            // Setup the container names
            this.SecurityServiceContainerName = $"securityservice{testGuid:N}";
            this.EstateManagementContainerName = $"estate{testGuid:N}";
            this.EventStoreContainerName = $"eventstore{testGuid:N}";
            this.EstateReportingContainerName = $"estatereporting{testGuid:N}";
            this.TransactionProcessorContainerName = $"txnprocessor{testGuid:N}";
            this.TransactionProcessorACLContainerName = $"txnprocessoracl{testGuid:N}";
            this.TestHostContainerName = $"testhosts{testGuid:N}";
            this.VoucherManagementContainerName = $"vouchermanagement{testGuid:N}";
            
            this.DockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");
            this.ClientDetails = ("serviceClient", "Secret1");
            this.SqlServerDetails = (Setup.SqlServerContainerName, Setup.SqlUserName, Setup.SqlPassword);

            INetworkService testNetwork = DockerHelper.SetupTestNetwork();
            this.TestNetworks.Add(testNetwork);

            IContainerService eventStoreContainer =
                this.SetupEventStoreContainer("eventstore/eventstore:21.10.0-buster-slim", testNetwork, isSecure: this.IsSecureEventStore);
            this.EventStoreHttpPort = eventStoreContainer.ToHostExposedEndpoint($"{DockerHelper.EventStoreHttpDockerPort}/tcp").Port;

            String insecureEventStoreEnvironmentVariable = "EventStoreSettings:Insecure=True";
            if (this.IsSecureEventStore)
            {
                insecureEventStoreEnvironmentVariable = "EventStoreSettings:Insecure=False";
            }

            String persistentSubscriptionPollingInSeconds = "AppSettings:PersistentSubscriptionPollingInSeconds=10";
            String internalSubscriptionServiceCacheDuration = "AppSettings:InternalSubscriptionServiceCacheDuration=0";

            IContainerService estateManagementContainer = this.SetupEstateManagementContainer("stuartferguson/estatemanagement",
                                                                                              new List<INetworkService>
                                                                                              {
                                                                                                  testNetwork,
                                                                                                  Setup.DatabaseServerNetwork
                                                                                              },
                                                                                              true,
                                                                                              additionalEnvironmentVariables:new List<String>
                                                                                                  {
                                                                                                      insecureEventStoreEnvironmentVariable,
                                                                                                      persistentSubscriptionPollingInSeconds,
                                                                                                      internalSubscriptionServiceCacheDuration
                                                                                                  });

            IContainerService securityServiceContainer = this.SetupSecurityServiceContainer("stuartferguson/securityservice",
                                                                                                    testNetwork,
                                                                                                    true);

            IContainerService voucherManagementContainer = this.SetupVoucherManagementContainer("stuartferguson/vouchermanagement",
                                                                                                new List<INetworkService>
                                                                                                {
                                                                                                    testNetwork
                                                                                                },
                                                                                                true,
                                                                                                additionalEnvironmentVariables:new List<String>
                                                                                                    {
                                                                                                        insecureEventStoreEnvironmentVariable,
                                                                                                    });

            String test = "OperatorConfiguration:PataPawaPostPay:Url=http://" + this.TestHostContainerName + ":9000/PataPawaPostPayService/basichttp";
            String test2 = "OperatorConfiguration:PataPawaPostPay:ApiLogonRequired=false";

            IContainerService transactionProcessorContainer = this.SetupTransactionProcessorContainer("stuartferguson/transactionprocessor",
                                                                                                      new List<INetworkService>
                                                                                                      {
                                                                                                          testNetwork
                                                                                                      },
                                                                                                      true,
                                                                                                      additionalEnvironmentVariables:new List<String>
                                                                                                          {
                                                                                                              test,
                                                                                                              test2,
                                                                                                              insecureEventStoreEnvironmentVariable,
                                                                                                              persistentSubscriptionPollingInSeconds,
                                                                                                              internalSubscriptionServiceCacheDuration,
                                                                                                              $"AppSettings:VoucherManagementApi=http://{this.VoucherManagementContainerName}:{DockerHelper.VoucherManagementDockerPort}"
                                                                                                          });

            IContainerService estateReportingContainer = this.SetupEstateReportingContainer("stuartferguson/estatereporting",
                                                                                            new List<INetworkService>
                                                                                            {
                                                                                                testNetwork,
                                                                                                Setup.DatabaseServerNetwork
                                                                                            },
                                                                                            true,
                                                                                            additionalEnvironmentVariables:new List<String>
                                                                                                {
                                                                                                    insecureEventStoreEnvironmentVariable,
                                                                                                    persistentSubscriptionPollingInSeconds,
                                                                                                    internalSubscriptionServiceCacheDuration
                                                                                                });

            List<String> additionalVariables = new List<String>()
                                               {
                                                   $"AppSettings:MinimumSupportedApplicationVersion={DockerHelper.MinimumSupportedApplicationVersion}"
                                               };

            IContainerService transactionProcessorACLContainer = this.SetupTransactionProcessorACLContainer("transactionprocessoracl", testNetwork,
                 additionalEnvironmentVariables:additionalVariables);

            IContainerService testhostContainer = this.SetupTestHostContainer("stuartferguson/testhosts",
                                                                         new List<INetworkService>
                                                                         {
                                                                             testNetwork,
                                                                             Setup.DatabaseServerNetwork
                                                                         },
                                                                         true);

            this.Containers.AddRange(new List<IContainerService>
                                     {
                                         eventStoreContainer,
                                         estateManagementContainer,
                                         securityServiceContainer,
                                         transactionProcessorContainer,
                                         transactionProcessorACLContainer,
                                         estateReportingContainer,
                                         testhostContainer,
                                         voucherManagementContainer
                                     });

            // Cache the ports
            this.EstateManagementApiPort = estateManagementContainer.ToHostExposedEndpoint("5000/tcp").Port;
            this.SecurityServicePort = securityServiceContainer.ToHostExposedEndpoint("5001/tcp").Port;
            this.TransactionProcessorPort = transactionProcessorContainer.ToHostExposedEndpoint("5002/tcp").Port;
            this.TransactionProcessorACLPort = transactionProcessorACLContainer.ToHostExposedEndpoint("5003/tcp").Port;

            // Setup the base address resolvers
            String EstateManagementBaseAddressResolver(String api) => $"http://127.0.0.1:{this.EstateManagementApiPort}";
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String TransactionProcessorBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TransactionProcessorPort}";
            String TransactionProcessorAclBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TransactionProcessorACLPort}";

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

            await this.LoadEventStoreProjections(this.EventStoreHttpPort, this.IsSecureEventStore).ConfigureAwait(false);
        }

        public async Task PopulateSubscriptionServiceConfiguration(String estateName, Boolean isSecureEventStore)
        {
            List<(String streamName, String groupName, Int32 maxRetries)> subscriptions = new List<(String streamName, String groupName, Int32 maxRetries)>();
            subscriptions.Add((estateName.Replace(" ", ""), "Reporting", 5));
            subscriptions.Add(($"EstateManagementSubscriptionStream_{estateName.Replace(" ", "")}", "Estate Management", 0));
            subscriptions.Add(($"TransactionProcessorSubscriptionStream_{estateName.Replace(" ", "")}", "Transaction Processor", 0));
            await this.PopulateSubscriptionServiceConfiguration(this.EventStoreHttpPort, subscriptions,
                                                                isSecureEventStore);
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

            if (this.Containers.Any())
            {
                foreach (IContainerService containerService in this.Containers)
                {
                    containerService.StopOnDispose = true;
                    containerService.RemoveOnDispose = true;
                    containerService.Dispose();
                }
            }

            if (this.TestNetworks.Any())
            {
                foreach (INetworkService networkService in this.TestNetworks)
                {
                    networkService.Stop();
                    networkService.Remove(true);
                }
            }
        }

        #endregion
    }
}