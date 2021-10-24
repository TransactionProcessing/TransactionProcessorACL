namespace TransactionProcessor.IntegrationTests.Common
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

        protected String SecurityServiceContainerName;

        protected String EstateManagementContainerName;

        protected String EventStoreContainerName;

        protected String EstateReportingContainerName;

        protected String SubscriptionServiceContainerName;

        protected String TransactionProcessorContainerName;

        protected String TransactionProcessorACLContainerName;

        protected String TestHostContainerName;

        public const Int32 TestHostPort = 9000;

        private const String MinimumSupportedApplicationVersion = "1.0.5";
        protected String VoucherManagementContainerName;
        
        /// <summary>
        /// The transaction processor port
        /// </summary>
        protected Int32 TransactionProcessorPort;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly NlogLogger Logger;

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

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName)
        {
            String traceFolder = FdOs.IsWindows() ? $"D:\\home\\txnproc\\trace\\{scenarioName}" : $"//home//txnproc//trace//{scenarioName}";

            Logging.Enabled();

            Guid testGuid = Guid.NewGuid();
            this.TestId = testGuid;

            this.Logger.LogInformation($"Test Id is {testGuid}");

            // Setup the container names
            this.SecurityServiceContainerName = $"securityservice{testGuid:N}";
            this.EstateManagementContainerName = $"estate{testGuid:N}";
            this.EventStoreContainerName = $"eventstore{testGuid:N}";
            this.EstateReportingContainerName = $"estatereporting{testGuid:N}";
            this.SubscriptionServiceContainerName = $"subscription{testGuid:N}";
            this.TransactionProcessorContainerName = $"txnprocessor{testGuid:N}";
            this.TransactionProcessorACLContainerName = $"txnprocessoracl{testGuid:N}";
            this.TestHostContainerName = $"testhosts{testGuid:N}";
            this.VoucherManagementContainerName = $"vouchermanagement{testGuid:N}";

            String eventStoreAddress = $"http://{this.EventStoreContainerName}";

            (String, String, String) dockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");

            INetworkService testNetwork = DockerHelper.SetupTestNetwork();
            this.TestNetworks.Add(testNetwork);
            IContainerService eventStoreContainer = DockerHelper.SetupEventStoreContainer(this.EventStoreContainerName, this.Logger, "eventstore/eventstore:20.10.0-buster-slim", testNetwork, traceFolder);
            this.EventStoreHttpPort = eventStoreContainer.ToHostExposedEndpoint($"{DockerHelper.EventStoreHttpDockerPort}/tcp").Port;
            
            IContainerService estateManagementContainer = DockerHelper.SetupEstateManagementContainer(this.EstateManagementContainerName, this.Logger,
                                                                                                      "stuartferguson/estatemanagement", new List<INetworkService>
                                                                                                                          {
                                                                                                                              testNetwork,
                                                                                                                              Setup.DatabaseServerNetwork
                                                                                                                          }, traceFolder, null,
                                                                                                      this.SecurityServiceContainerName,
                                                                                                      eventStoreAddress,
                                                                                                      (Setup.SqlServerContainerName,
                                                                                                      "sa",
                                                                                                      "thisisalongpassword123!"),
                                                                                                      ("serviceClient", "Secret1"),
                                                                                                      true);

            IContainerService securityServiceContainer = DockerHelper.SetupSecurityServiceContainer(this.SecurityServiceContainerName,
                                                                                                    this.Logger,
                                                                                                    "stuartferguson/securityservice",
                                                                                                    testNetwork,
                                                                                                    traceFolder,
                                                                                                    dockerCredentials,
                                                                                                    true);

            IContainerService voucherManagementContainer = DockerHelper.SetupVoucherManagementContainer(this.VoucherManagementContainerName,
                                                                                                        this.Logger,
                                                                                                        "stuartferguson/vouchermanagement",
                                                                                                        new List<INetworkService>
                                                                                                        {
                                                                                                            testNetwork
                                                                                                        },
                                                                                                        traceFolder,
                                                                                                        dockerCredentials,
                                                                                                        this.SecurityServiceContainerName,
                                                                                                        this.EstateManagementContainerName,
                                                                                                        eventStoreAddress,
                                                                                                        (Setup.SqlServerContainerName,
                                                                                                            "sa",
                                                                                                            "thisisalongpassword123!"),
                                                                                                        ("serviceClient", "Secret1"),
                                                                                                        true);
            
            IContainerService transactionProcessorContainer = DockerHelper.SetupTransactionProcessorContainer(this.TransactionProcessorContainerName,
                                                                                                              this.Logger,
                                                                                                              "stuartferguson/transactionprocessor",
                                                                                                              new List<INetworkService>
                                                                                                              {
                                                                                                                  testNetwork
                                                                                                              },
                                                                                                              traceFolder,
                                                                                                              dockerCredentials,
                                                                                                              this.SecurityServiceContainerName,
                                                                                                              this.EstateManagementContainerName,
                                                                                                              eventStoreAddress,
                                                                                                              ("serviceClient", "Secret1"),
                                                                                                              this.TestHostContainerName,
                                                                                                              this.VoucherManagementContainerName,
                                                                                                              true);

            IContainerService estateReportingContainer = DockerHelper.SetupEstateReportingContainer(this.EstateReportingContainerName,
                                                                                                    this.Logger,
                                                                                                    "stuartferguson/estatereporting",
                                                                                                    new List<INetworkService>
                                                                                                    {
                                                                                                        testNetwork,
                                                                                                        Setup.DatabaseServerNetwork
                                                                                                    },
                                                                                                    traceFolder,
                                                                                                    dockerCredentials,
                                                                                                    this.SecurityServiceContainerName,
                                                                                                    eventStoreAddress,
                                                                                                    (Setup.SqlServerContainerName,
                                                                                                    "sa",
                                                                                                    "thisisalongpassword123!"),
                                                                                                    ("serviceClient", "Secret1"),
                                                                                                    true);

            List<String> additionalVariables = new List<String>()
                                               {
                                                   $"AppSettings:MinimumSupportedApplicationVersion={DockerHelper.MinimumSupportedApplicationVersion}"
                                               };

            IContainerService transactionProcessorACLContainer = DockerHelper.SetupTransactionProcessorACLContainer(this.TransactionProcessorACLContainerName,
                                                                                                                    this.Logger,
                                                                                                                    "transactionprocessoracl",
                                                                                                                    testNetwork,
                                                                                                                    traceFolder,
                                                                                                                    null,
                                                                                                                    this.SecurityServiceContainerName, 
                                                                                                                    this.TransactionProcessorContainerName,
                                                                                                                    ("serviceClient", "Secret1"),
                 additionalEnvironmentVariables:additionalVariables);

            IContainerService testhostContainer = DockerHelper.SetupTestHostContainer(this.TestHostContainerName,
                                                                         this.Logger,
                                                                         "stuartferguson/testhosts",
                                                                         new List<INetworkService>
                                                                         {
                                                                             testNetwork,
                                                                             Setup.DatabaseServerNetwork
                                                                         },
                                                                         traceFolder,
                                                                         dockerCredentials, 
                                                                         (Setup.SqlServerContainerName,
                                                                             "sa",
                                                                             "thisisalongpassword123!"),
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

            await this.LoadEventStoreProjections().ConfigureAwait(false);
        }

        private static EventStoreClientSettings ConfigureEventStoreSettings(Int32 eventStoreHttpPort)
        {
            String connectionString = $"http://127.0.0.1:{eventStoreHttpPort}";

            EventStoreClientSettings settings = new EventStoreClientSettings();
            settings.CreateHttpMessageHandler = () => new SocketsHttpHandler
                                                      {
                                                          SslOptions =
                                                          {
                                                              RemoteCertificateValidationCallback = (sender,
                                                                                                     certificate,
                                                                                                     chain,
                                                                                                     errors) => true,
                                                          }
                                                      };
            settings.ConnectionName = "Specflow";
            settings.ConnectivitySettings = new EventStoreClientConnectivitySettings
                                            {
                                                Address = new Uri(connectionString),
                                            };

            settings.DefaultCredentials = new UserCredentials("admin", "changeit");
            return settings;
        }

        private async Task LoadEventStoreProjections()
        {
            //Start our Continous Projections - we might decide to do this at a different stage, but now lets try here
            String projectionsFolder = "../../../projections/continuous";
            IPAddress[] ipAddresses = Dns.GetHostAddresses("127.0.0.1");

            if (!String.IsNullOrWhiteSpace(projectionsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(projectionsFolder);

                if (di.Exists)
                {
                    FileInfo[] files = di.GetFiles();

                    EventStoreProjectionManagementClient projectionClient = new EventStoreProjectionManagementClient(ConfigureEventStoreSettings(this.EventStoreHttpPort));

                    foreach (FileInfo file in files)
                    {
                        String projection = File.ReadAllText(file.FullName);
                        String projectionName = file.Name.Replace(".js", String.Empty);

                        try
                        {
                            Logger.LogInformation($"Creating projection [{projectionName}]");
                            await projectionClient.CreateContinuousAsync(projectionName, projection, trackEmittedStreams:true).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(new Exception($"Projection [{projectionName}] error", e));
                        }
                    }
                }
            }

            Logger.LogInformation("Loaded projections");
        }

        public async Task PopulateSubscriptionServiceConfiguration(String estateName)
        {
            EventStorePersistentSubscriptionsClient client = new EventStorePersistentSubscriptionsClient(ConfigureEventStoreSettings(this.EventStoreHttpPort));

            PersistentSubscriptionSettings settings = new PersistentSubscriptionSettings(resolveLinkTos: true, StreamPosition.Start);
            await client.CreateAsync(estateName.Replace(" ", ""), "Reporting", settings);
            await client.CreateAsync($"EstateManagementSubscriptionStream_{estateName.Replace(" ", "")}", "Estate Management", settings);
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
                                    EstateReportingContext context = new EstateReportingContext(connectionString);
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