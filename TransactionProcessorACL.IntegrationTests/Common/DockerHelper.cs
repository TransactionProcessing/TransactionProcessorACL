using DotNet.Testcontainers.Builders;
using EventStore.Client;
using KurrentDB.Client;
using Shared.IntegrationTesting;
using System.Linq;

namespace TransactionProcessor.IntegrationTests.Common
{
    using Client;
    using SecurityService.Client;
    using Shared.IntegrationTesting.TestContainers;
    using Shared.Serialisation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using TestHosts.Clients;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.IntegrationTesting.DockerHelper" />
    public class DockerHelper : global::Shared.IntegrationTesting.TestContainers.DockerHelper
    {
        #region Fields

        /// <summary>
        /// The HTTP client
        /// </summary>
        public HttpClient HttpClient;

        public HttpClient TestHostHttpClient;

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

        public KurrentDBProjectionManagementClient ProjectionManagementClient;

        public IAgencyBankingClient AgencyBankingClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DockerHelper()
        {
            this.IsSecureKurrentDb = false;
            this.TestingContext = new TestingContext();
            StringSerialiser.Initialise((IStringSerialiser)new SystemTextJsonSerializer(SystemTextJsonSerializer.GetDefaultJsonSerializerOptions()));
        }

        #endregion

        #region Methods

        public override ContainerBuilder SetupKurrentDbContainer()
        {
            this.Trace("About to Start Kurrent Db Container");

            Dictionary<String, String> environmentVariables = new(){
            {"EVENTSTORE_RUN_PROJECTIONS","all"},
            {"EVENTSTORE_START_STANDARD_PROJECTIONS","true"},
            {"EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP","true"},
            {"EVENTSTORE_PROJECTION_EXECUTION_TIMEOUT","5000"},
            {"KURRENTDB_PREPARE_TIMEOUT_MS","30000"},
            {"KURRENTDB_COMMIT_TIMEOUT_MS","30000"},
        };
            ContainerBuilder eventStoreContainer = new();

            if (!this.IsSecureKurrentDb)
            {
                environmentVariables.Add("EVENTSTORE_INSECURE", "true");
            }
            else
            {
                String certsPath = "/etc/eventstore/certs";

                // Copy these to the container
                String path = Path.Combine(Directory.GetCurrentDirectory(), "certs");

                eventStoreContainer = eventStoreContainer.MountHostFolder(path, certsPath);

                // Certificates configuration
                environmentVariables.Add("EVENTSTORE_CertificateFile", $"{certsPath}/node1/node.crt");
                environmentVariables.Add("EVENTSTORE_CertificatePrivateKeyFile", $"{certsPath}/node1/node.key");
                environmentVariables.Add("EVENTSTORE_TrustedRootCertificatesPath", $"{certsPath}/ca");
                environmentVariables.Add("EVENTSTORE_INSECURE", "false");
            }

            String logfilePath = "/var/log/kurrentdb";
            eventStoreContainer = eventStoreContainer.MountHostFolder(this.HostTraceFolder, logfilePath);

            (String imageName, Boolean useLatest) imageDetails = this.GetImageDetails(ContainerType.KurrentDb).Data;
            this.Trace($"About to Start Kurrent Db Container using image [{imageDetails.imageName}]");



            eventStoreContainer = eventStoreContainer.WithName(this.KurrentDbContainerName)  // similar to WithName()
                .WithImage(imageDetails.imageName)
                .WithEnvironment(environmentVariables)
                .WithPortBinding(DockerPorts.KurrentDbHttpDockerPort, true);

            return eventStoreContainer;
        }

        public override DotNet.Testcontainers.Builders.ContainerBuilder SetupEstateReportingContainer()
        {
            Dictionary<String, String> environmentVariables = new();
            environmentVariables.Add("ConnectionStrings:TransactionProcessorReadModel", this.SetConnectionString("TransactionProcessorReadModel", this.UseSecureSqlServerDatabase));
            this.AdditionalVariables.Add(ContainerType.EstateReporting, environmentVariables);

            return base.SetupEstateReportingContainer();
        }

        //public override ContainerBuilder SetupSecurityServiceContainer()
        //{
        //    Dictionary<String, String> additionalVariables = new();
        //    additionalVariables.Add("ServiceOptions:OAuth:LegacyGrantTypeClients:password:1", "estateClient");

        //    this.AdditionalVariables.Add(ContainerType.SecurityService, additionalVariables);

        //    return base.SetupSecurityServiceContainer();
        //}

        public override ContainerBuilder SetupTransactionProcessorContainer()
        {
            Dictionary<String, String> additionalVariables = new();
            additionalVariables.Add("OperatorConfiguration:AgencyBanking:Url", $"http://{this.TestHostContainerName}:{DockerPorts.TestHostPort}/api/agencybanking");            
            this.AdditionalVariables.Add(ContainerType.TransactionProcessor, additionalVariables);

            return base.SetupTransactionProcessorContainer();
        }

        protected override String GenerateKurrentDbConnectionString(String userName = "admin", String password = "changeit")
        {
            String eventStoreAddress = $"esdb://{userName}:{password}@{this.KurrentDbContainerName}:{DockerPorts.KurrentDbHttpDockerPort}?tls=false&defaultDeadline=240000";

            return eventStoreAddress;
        }

        public override async Task CreateSubscriptions(CancellationToken cancellationToken){
            List<(String streamName, String groupName, Int32 maxRetries)> subscriptions = new List<(String streamName, String groupName, Int32 maxRetries)>();
            subscriptions.AddRange(MessagingService.IntegrationTesting.Helpers.SubscriptionsHelper.GetSubscriptions());
            subscriptions.AddRange(TransactionProcessor.IntegrationTesting.Helpers.SubscriptionsHelper.GetSubscriptions());

            foreach ((String streamName, String groupName, Int32 maxRetries) subscription in subscriptions)
            {
                var x = subscription;
                x.maxRetries = 2;
                await this.CreatePersistentSubscription(x);
            }
        }

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName, DockerServices dockerServices, CancellationToken cancellationToken)
        {
            try
            {
                await base.StartContainersForScenarioRun(scenarioName, dockerServices, cancellationToken);
            }
            catch
            {
                this.DumpDockerContainerState();
                throw;
            }
        
            // Setup the base address resolvers
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String TransactionProcessorBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TransactionProcessorPort}";
            String TransactionProcessorAclBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TransactionProcessorAclPort}";
            String TestHostServiceBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TestHostServicePort}";
            
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
            this.SecurityServiceClient = new SecurityServiceClient(SecurityServiceBaseAddressResolver, httpClient, Serialise, Deserialise);
            this.TransactionProcessorClient = new TransactionProcessorClient(TransactionProcessorBaseAddressResolver, httpClient, Serialise, Deserialise);
            this.TestHostHttpClient = new HttpClient(clientHandler);
            this.TestHostHttpClient.BaseAddress = new Uri($"http://127.0.0.1:{this.TestHostServicePort}");
            this.AgencyBankingClient = new AgencyBankingClient(TestHostServiceBaseAddressResolver, httpClient, Serialise_CamelCase, this.Deserialise_CamelCase);

            this.HttpClient = new HttpClient();
            this.HttpClient.BaseAddress = new Uri(TransactionProcessorAclBaseAddressResolver(string.Empty));
            this.HttpClient.Timeout = TimeSpan.FromMinutes(5);

            this.ProjectionManagementClient = new KurrentDBProjectionManagementClient(this.ConfigureKurrentDbSettings());

            SimpleResults.Result<SecurityService.DataTransferObjects.TokenResponse> bootstrapToken = await this.SecurityServiceClient.GetToken("management-bootstrap", "management-bootstrap-secret", CancellationToken.None);
            if (bootstrapToken.IsFailed || String.IsNullOrWhiteSpace(bootstrapToken.Data?.AccessToken))
            {
                throw new InvalidOperationException("Unable to obtain the integration-test management bootstrap token.");
            }
            this.AccessToken = bootstrapToken.Data.AccessToken;
        }

        public String AccessToken;

        private void DumpDockerContainerState()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "ps -a --no-trunc --format \"table {{.ID}}\\t{{.Names}}\\t{{.Status}}\\t{{.Image}}\\t{{.Ports}}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo);
            if (process == null)
            {
                Console.WriteLine("Unable to start docker diagnostics process.");
                return;
            }

            String output = process.StandardOutput.ReadToEnd();
            String error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine("Docker containers at container startup failure:");
            Console.WriteLine(output);
            if (String.IsNullOrWhiteSpace(error) == false)
            {
                Console.WriteLine("Docker diagnostics error:");
                Console.WriteLine(error);
            }
        }

        String Serialise(Object arg)
        {
            return StringSerialiser.Serialise<Object>(arg, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        }

        Object Deserialise(String arg, Type type)
        {
            return StringSerialiser.DeserializeObject<Object>(arg, type, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        }

        String Serialise_CamelCase(Object arg)
        {
            return StringSerialiser.Serialise<Object>(arg, new SerialiserOptions(SerialiserPropertyFormat.CamelCase));
        }

        Object Deserialise_CamelCase(String arg, Type type)
        {
            return StringSerialiser.DeserializeObject<Object>(arg, type, new SerialiserOptions(SerialiserPropertyFormat.CamelCase));
        }

        public override Dictionary<String, String> GetAdditionalVariables(ContainerType containerType)
        {
            Dictionary<String, String> additionalVariables = base.GetAdditionalVariables(containerType);

            if (containerType == ContainerType.TransactionProcessorAcl &&
                (this.RequiredDockerServices & DockerServices.EstateReporting) == DockerServices.EstateReporting)
            {
                additionalVariables["AppSettings:EstateReportingApi"] =
                    $"http://{this.EstateReportingContainerName}:{DockerPorts.EstateReportingDockerPort}";
            }

            if (containerType == ContainerType.TransactionProcessorAcl)
            {
                additionalVariables["Logging:LogLevel:Microsoft.AspNetCore.Authorization"] = "Debug";
            }

            return additionalVariables;
        }

        public override ContainerBuilder SetupSecurityServiceContainer()
        {
            Dictionary<String, String> additionalVariables = new();
            additionalVariables.Add("ServiceOptions:OAuth:LegacyGrantTypeClients:password:1", "estateClient");
            additionalVariables.Add("ServiceOptions:OAuth:LegacyGrantTypeClients:password:2", "redemptionClient");
            additionalVariables.Add("ServiceOptions:UserOptions:UseProvidedPasswordForEmailUsers", "true");

            this.AdditionalVariables.Add(ContainerType.SecurityService, additionalVariables);

            return base.SetupSecurityServiceContainer();
        }

        #endregion
    }


}
