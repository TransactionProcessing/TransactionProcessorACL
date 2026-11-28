using DotNet.Testcontainers.Builders;
using EventStore.Client;
using KurrentDB.Client;
using SecurityService.DataTransferObjects;
using Shouldly;
using System.Linq;
using System.Text.Json;
using TransactionProcessing.Shared.IntegrationTesting;
using TransactionProcessing.Shared.IntegrationTesting.TestContainers;
using TransactionProcessing.Shared.Serialisation;
using TransactionProcessing.UnionResults;

namespace TransactionProcessor.IntegrationTests.Common {
    using Client;
    using SecurityService.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using TestHosts.Clients;
    using DotNet.Testcontainers.Networks;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.IntegrationTesting.DockerHelper" />
    public class DockerHelper : global::TransactionProcessing.Shared.IntegrationTesting.TestContainers.DockerHelper {
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
        public DockerHelper() {
            this.IsSecureKurrentDb = false;
            this.TestingContext = new TestingContext();
            StringSerialiser.Initialise((IStringSerialiser)new SystemTextJsonSerializer(SystemTextJsonSerializer.GetDefaultJsonSerializerOptions()));
        }

        #endregion

        #region Methods

        public override ContainerBuilder SetupKurrentDbContainer() {
            this.Trace("About to Start Kurrent Db Container");

            (String imageName, Boolean useLatest) imageDetails = this.GetImageDetails(ContainerType.KurrentDb).Value.ShouldBeOfType<Success<(String imageName, Boolean useLatest)>>().Value;

            Dictionary<String, String> environmentVariables = new(){
            {"EVENTSTORE_RUN_PROJECTIONS","all"},
            {"EVENTSTORE_START_STANDARD_PROJECTIONS","true"},
            {"EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP","true"},
            {"EVENTSTORE_PROJECTION_EXECUTION_TIMEOUT","5000"},
            {"KURRENTDB_PREPARE_TIMEOUT_MS","30000"},
            {"KURRENTDB_COMMIT_TIMEOUT_MS","30000"},
        };
            ContainerBuilder eventStoreContainer = new(imageDetails.imageName);

            if (!this.IsSecureKurrentDb) {
                environmentVariables.Add("EVENTSTORE_INSECURE", "true");
            }
            else {
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


            this.Trace($"About to Start Kurrent Db Container using image [{imageDetails.imageName}]");



            eventStoreContainer = eventStoreContainer.WithName(this.KurrentDbContainerName)  // similar to WithName()
                .WithEnvironment(environmentVariables)
                .WithPortBinding(DockerPorts.KurrentDbHttpDockerPort, true);

            return eventStoreContainer;
        }

        public override ContainerBuilder SetupTransactionProcessorAclContainer() {
            this.Trace("About to Start Transaction Processor ACL Container");

            IntegrationTestCertificate certificate = this.IntegrationTestCertificate
                ?? throw new InvalidOperationException("The integration test certificate was not initialized before ACL container setup.");

            Dictionary<String, String> environmentVariables = this.GetCommonEnvironmentVariables();
            environmentVariables.Add("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
            environmentVariables.Add("urls", $"https://*:{DockerPorts.TransactionProcessorAclDockerPort}");
            environmentVariables.Add("ServiceOptions__KestrelOptions__Path", certificate.GetContainerCertificatePath());
            environmentVariables.Add("ServiceOptions__KestrelOptions__Password", certificate.Password);

            Dictionary<String, String> additionalEnvironmentVariables = this.GetAdditionalVariables(ContainerType.TransactionProcessorAcl);
            foreach (KeyValuePair<String, String> additionalEnvironmentVariable in additionalEnvironmentVariables) {
                environmentVariables.Add(additionalEnvironmentVariable.Key, additionalEnvironmentVariable.Value);
            }

            (String imageName, Boolean useLatest) imageDetails = this.GetImageDetails(ContainerType.TransactionProcessorAcl)
                .Value.ShouldBeOfType<Success<(String imageName, Boolean useLatest)>>().Value;

            return new ContainerBuilder(imageDetails.imageName)
                .WithName(this.TransactionProcessorAclContainerName)
                .WithEnvironment(environmentVariables)
                .MountHostFolder(this.HostTraceFolder)
                .MountHostFolder(certificate.CertificateDirectory, certificate.ContainerCertificateDirectory)
                .WithPortBinding(DockerPorts.TransactionProcessorAclDockerPort, true);
        }

        public override DotNet.Testcontainers.Builders.ContainerBuilder SetupEstateReportingContainer() {
            Dictionary<String, String> environmentVariables = new();
            environmentVariables.Add("ConnectionStrings:TransactionProcessorReadModel", this.SetConnectionString("TransactionProcessorReadModel", this.UseSecureSqlServerDatabase));
            this.AdditionalVariables.Add(ContainerType.EstateReporting, environmentVariables);

            return this.ConfigureHttpsContainer(
                base.SetupEstateReportingContainer(),
                DockerPorts.EstateReportingDockerPort);
        }

        //public override ContainerBuilder SetupSecurityServiceContainer()
        //{
        //    Dictionary<String, String> additionalVariables = new();
        //    additionalVariables.Add("ServiceOptions:OAuth:LegacyGrantTypeClients:password:1", "estateClient");

        //    this.AdditionalVariables.Add(ContainerType.SecurityService, additionalVariables);

        //    return base.SetupSecurityServiceContainer();
        //}

        public override ContainerBuilder SetupTransactionProcessorContainer() {
            Dictionary<String, String> additionalVariables = new();
            additionalVariables.Add("OperatorConfiguration:AgencyBanking:Url", $"http://{this.TestHostContainerName}:{DockerPorts.TestHostPort}/api/agencybanking");
            this.AdditionalVariables.Add(ContainerType.TransactionProcessor, additionalVariables);

            return this.ConfigureHttpsContainer(
                base.SetupTransactionProcessorContainer(),
                DockerPorts.TransactionProcessorDockerPort);
        }

        public override ContainerBuilder SetupCallbackHandlerContainer() =>
            this.ConfigureHttpsContainer(base.SetupCallbackHandlerContainer(), DockerPorts.CallbackHandlerDockerPort);

        public override ContainerBuilder SetupMessagingServiceContainer() =>
            this.ConfigureHttpsContainer(base.SetupMessagingServiceContainer(), DockerPorts.MessagingServiceDockerPort);

        private ContainerBuilder ConfigureHttpsContainer(ContainerBuilder container, Int32 port) {
            IntegrationTestCertificate certificate = this.IntegrationTestCertificate
                ?? throw new InvalidOperationException("The integration test certificate was not initialized before container setup.");

            return container
                .WithEnvironment("urls", $"https://*:{port}")
                .WithEnvironment("ServiceOptions__KestrelOptions__Path", certificate.GetContainerCertificatePath())
                .WithEnvironment("ServiceOptions__KestrelOptions__Password", certificate.Password)
                .WithEnvironment("HealthMonitoring__Service__HealthUrl", $"https://localhost:{port}/healthui")
                .MountHostFolder(certificate.CertificateDirectory, certificate.ContainerCertificateDirectory);
        }

        protected override String GenerateKurrentDbConnectionString(String userName = "admin", String password = "changeit") {
            String eventStoreAddress = $"esdb://{userName}:{password}@{this.KurrentDbContainerName}:{DockerPorts.KurrentDbHttpDockerPort}?tls=false&defaultDeadline=240000";

            return eventStoreAddress;
        }

        public override async Task CreateSubscriptions(CancellationToken cancellationToken) {
            List<(String streamName, String groupName, Int32 maxRetries)> subscriptions = new List<(String streamName, String groupName, Int32 maxRetries)>();
            subscriptions.AddRange(MessagingService.IntegrationTesting.Helpers.SubscriptionsHelper.GetSubscriptions());
            subscriptions.AddRange(TransactionProcessor.IntegrationTesting.Helpers.SubscriptionsHelper.GetSubscriptions());

            foreach ((String streamName, String groupName, Int32 maxRetries) subscription in subscriptions) {
                var x = subscription;
                x.maxRetries = 2;
                await this.CreatePersistentSubscription(x);
            }
        }

        public override Dictionary<String, String> GetCommonEnvironmentVariables() {
            Dictionary<String, String> environmentVariables = base.GetCommonEnvironmentVariables();
            environmentVariables["AppSettings:TransactionProcessorApi"] =
                $"https://{this.TransactionProcessorContainerName}:{DockerPorts.TransactionProcessorDockerPort}";
            environmentVariables["AppSettings:MessagingServiceApi"] =
                $"https://{this.MessagingServiceContainerName}:{DockerPorts.MessagingServiceDockerPort}";
            return environmentVariables;
        }

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName, DockerServices dockerServices, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            this.IntegrationTestCertificate = IntegrationTestCertificate.Create();
            try {
                this.RequiredDockerServices = dockerServices;
                this.IsSecureKurrentDb = Environment.GetEnvironmentVariable("IsSecureEventStore") switch {
                    null => false,
                    { Length: 0 } => false,
                    "false" => false,
                    _ => true
                };
                this.SetHostTraceFolder(scenarioName, this.SharedSqlServerFixture?.StartedAt);
                this.TestId = Guid.NewGuid();
                this.Trace($"Test Id is {this.TestId}");
                this.SetupContainerNames();
                if (this.SharedSqlServerFixture is not null) {
                    this.SqlServerContainerName = this.SharedSqlServerFixture.ContainerName;
                }

                this.ClientDetails = ("serviceClient", "Secret1");

                INetwork testNetwork;
                if (this.SharedSqlServerFixture is null) {
                    testNetwork = await this.SetupTestNetwork();
                    this.TestNetworks.Add(testNetwork);
                } else {
                    testNetwork = this.SharedSqlServerFixture.Network;
                }

                List<INetwork> networks = [testNetwork];

                Boolean RequiresLocalHttpsHealthCheck(DockerServices service) => service is
                    DockerServices.CallbackHandler or
                    DockerServices.MessagingService or
                    DockerServices.TransactionProcessor or
                    DockerServices.EstateReporting or
                    DockerServices.TransactionProcessorAcl;

                Int32 GetHttpsPort(DockerServices service) => service switch {
                    DockerServices.CallbackHandler => this.CallbackHandlerPort,
                    DockerServices.MessagingService => this.MessagingServicePort,
                    DockerServices.TransactionProcessor => this.TransactionProcessorPort,
                    DockerServices.EstateReporting => this.EstateReportingPort,
                    DockerServices.TransactionProcessorAcl => this.TransactionProcessorAclPort,
                    _ => throw new InvalidOperationException($"No HTTPS health check port exists for [{service}].")
                };

                async Task StartWithTrace(Func<ContainerBuilder> setupContainer, DockerServices service) {
                    DateTimeOffset startedAt = DateTimeOffset.UtcNow;
                    this.Trace($"Starting container [{service}]");

                    if (RequiresLocalHttpsHealthCheck(service)) {
                        Boolean skipHealthChecks = this.SkipHealthChecks;
                        this.SkipHealthChecks = true;
                        try {
                            await this.StartContainer2(setupContainer, networks, service, cancellationToken);
                        } finally {
                            this.SkipHealthChecks = skipHealthChecks;
                        }

                        await this.WaitForHttpsHealthCheck(service, GetHttpsPort(service), cancellationToken);
                    } else {
                        await this.StartContainer2(setupContainer, networks, service, cancellationToken);
                    }

                    TimeSpan elapsed = DateTimeOffset.UtcNow - startedAt;
                    this.Trace($"Container [{service}] started in {elapsed.TotalSeconds:N1}s");
                }

                Func<DockerServices, Func<ContainerBuilder>> setupContainerForService = service => service switch {
                    DockerServices.SqlServer => this.ConfigureSqlContainer,
                    DockerServices.KurrentDb => this.SetupKurrentDbContainer,
                    DockerServices.MessagingService => this.SetupMessagingServiceContainer,
                    DockerServices.SecurityService => this.SetupSecurityServiceContainer,
                    DockerServices.CallbackHandler => this.SetupCallbackHandlerContainer,
                    DockerServices.TestHost => this.SetupTestHostContainer,
                    DockerServices.TransactionProcessor => this.SetupTransactionProcessorContainer,
                    DockerServices.FileProcessor => this.SetupFileProcessorContainer,
                    DockerServices.TransactionProcessorAcl => this.SetupTransactionProcessorAclContainer,
                    DockerServices.ConfigurationHost => this.SetupConfigHostContainer,
                    DockerServices.EstateManagementUI => this.SetupEstateManagementUiContainer,
                    DockerServices.EstateReporting => this.SetupEstateReportingContainer,
                    DockerServices.SubscriptionService => this.SetupSubscriptionServiceContainer,
                    _ => throw new InvalidOperationException($"No startup plan exists for docker service [{service}]")
                };

                DockerServices servicesForStartup = GetServicesForStartup(
                    this.RequiredDockerServices,
                    this.SharedSqlServerFixture is not null);

                foreach (IReadOnlyList<DockerServices> startupGroup in this.GetStartupGroups()) {
                    cancellationToken.ThrowIfCancellationRequested();
                    List<DockerServices> services = startupGroup
                        .Where(service => (servicesForStartup & service) == service)
                        .ToList();

                    await Task.WhenAll(services
                        .Where(service => !RequiresLocalHttpsHealthCheck(service))
                        .Select(service => StartWithTrace(setupContainerForService(service), service)));

                    foreach (DockerServices service in services.Where(RequiresLocalHttpsHealthCheck)) {
                        await StartWithTrace(
                            setupContainerForService(service),
                            service);
                    }
                }

                await this.LoadEventStoreProjections(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                await this.CreateSubscriptions(cancellationToken);
            }
            catch (Exception startupException) {
                this.DumpDockerContainerState();
                try {
                    await this.StopContainersForScenarioRun(DockerServices.None).ConfigureAwait(false);
                } catch (Exception cleanupException) {
                    this.DisposeIntegrationTestCertificate();
                    throw new AggregateException(
                        "Scenario container startup failed and cleanup also failed.",
                        startupException,
                        cleanupException);
                }

                this.DisposeIntegrationTestCertificate();
                throw;
            }

            // Setup the base address resolvers
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String TransactionProcessorBaseAddressResolver(String api) => $"https://127.0.0.1:{this.TransactionProcessorPort}";
            String TransactionProcessorAclBaseAddressResolver(String api) => $"https://127.0.0.1:{this.TransactionProcessorAclPort}";
            String TestHostServiceBaseAddressResolver(String api) => $"http://127.0.0.1:{this.TestHostServicePort}";

            HttpClientHandler clientHandler = new HttpClientHandler {
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

            this.HttpClient = new HttpClient(clientHandler);
            this.HttpClient.BaseAddress = new Uri(TransactionProcessorAclBaseAddressResolver(string.Empty));
            this.HttpClient.Timeout = TimeSpan.FromMinutes(5);

            this.ProjectionManagementClient = new KurrentDBProjectionManagementClient(this.ConfigureKurrentDbSettings());

            Result<TokenResponse> bootstrapToken = await this.SecurityServiceClient.GetToken("management-bootstrap", "management-bootstrap-secret", CancellationToken.None);
            if (bootstrapToken.Value is not Success<TokenResponse> token || String.IsNullOrWhiteSpace(token.Value?.AccessToken)) {
                throw new InvalidOperationException("Unable to obtain the integration-test management bootstrap token.");
            }
            this.AccessToken = token.Value.AccessToken;
        }

        public String AccessToken;

        private async Task WaitForHttpsHealthCheck(DockerServices service, Int32 port, CancellationToken cancellationToken) {
            DateTimeOffset deadline = DateTimeOffset.UtcNow.AddMinutes(3);
            Exception lastException = null;

            while (DateTimeOffset.UtcNow < deadline) {
                cancellationToken.ThrowIfCancellationRequested();
                try {
                    Result<String> result = await this.HealthCheckClient.PerformHealthCheck(
                        "https",
                        "127.0.0.1",
                        port,
                        cancellationToken);

                    if (result.Value is not Success<String> success) {
                        String message = result.GetErrors().FirstOrDefault()?.Message ?? "Health check returned no result.";
                        throw new InvalidOperationException($"HTTPS health check failed for {service} [{message}]");
                    }

                    using JsonDocument healthResponse = JsonDocument.Parse(success.Value);
                    String status = healthResponse.RootElement.GetProperty("status").GetString();
                    if (status == "Healthy") {
                        return;
                    }

                        throw new InvalidOperationException($"{service} health status was [{status}]");
                } catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                    throw;
                } catch (Exception exception) {
                    lastException = exception;
                }

                TimeSpan remaining = deadline - DateTimeOffset.UtcNow;
                if (remaining > TimeSpan.Zero) {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Min(20, remaining.TotalSeconds)), cancellationToken);
                }
            }

            throw new InvalidOperationException($"{service} did not become healthy over HTTPS within three minutes.", lastException);
        }

        private void DumpDockerContainerState() {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = "docker",
                Arguments = "ps -a --no-trunc --format \"table {{.ID}}\\t{{.Names}}\\t{{.Status}}\\t{{.Image}}\\t{{.Ports}}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo);
            if (process == null) {
                Console.WriteLine("Unable to start docker diagnostics process.");
                return;
            }

            String output = process.StandardOutput.ReadToEnd();
            String error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine("Docker containers at container startup failure:");
            Console.WriteLine(output);
            if (String.IsNullOrWhiteSpace(error) == false) {
                Console.WriteLine("Docker diagnostics error:");
                Console.WriteLine(error);
            }
        }

        String Serialise(Object arg) {
            return StringSerialiser.Serialise<Object>(arg, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        }

        Object Deserialise(String arg, Type type) {
            return StringSerialiser.DeserializeObject<Object>(arg, type, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        }

        String Serialise_CamelCase(Object arg) {
            return StringSerialiser.Serialise<Object>(arg, new SerialiserOptions(SerialiserPropertyFormat.CamelCase));
        }

        Object Deserialise_CamelCase(String arg, Type type) {
            return StringSerialiser.DeserializeObject<Object>(arg, type, new SerialiserOptions(SerialiserPropertyFormat.CamelCase));
        }

        public override Dictionary<String, String> GetAdditionalVariables(ContainerType containerType) {
            Dictionary<String, String> additionalVariables = base.GetAdditionalVariables(containerType);

            if (containerType == ContainerType.TransactionProcessorAcl &&
                (this.RequiredDockerServices & DockerServices.EstateReporting) == DockerServices.EstateReporting) {
                additionalVariables["AppSettings:EstateReportingApi"] =
                    $"https://{this.EstateReportingContainerName}:{DockerPorts.EstateReportingDockerPort}";
            }

            if (containerType == ContainerType.TransactionProcessorAcl) {
                additionalVariables["Logging:LogLevel:Microsoft.AspNetCore.Authorization"] = "Debug";
            }

            return additionalVariables;
        }

        public override ContainerBuilder SetupSecurityServiceContainer() {
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
