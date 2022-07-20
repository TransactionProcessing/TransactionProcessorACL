namespace TransactionProcessor.IntegrationTests.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using EstateManagement.DataTransferObjects;
    using EstateManagement.DataTransferObjects.Requests;
    using EstateManagement.DataTransferObjects.Responses;
    using Newtonsoft.Json;
    using SecurityService.DataTransferObjects;
    using SecurityService.DataTransferObjects.Requests;
    using SecurityService.DataTransferObjects.Responses;
    using Shouldly;
    using TechTalk.SpecFlow;
    using TransactionProcessorACL.DataTransferObjects;
    using TransactionProcessorACL.DataTransferObjects.Responses;
    using ClientDetails = Common.ClientDetails;

    /// <summary>
    /// 
    /// </summary>
    [Binding]
    [Scope(Tag = "shared")]
    public class SharedSteps
    {
        #region Fields

        /// <summary>
        /// The scenario context
        /// </summary>
        private readonly ScenarioContext ScenarioContext;

        /// <summary>
        /// The testing context
        /// </summary>
        private readonly TestingContext TestingContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedSteps"/> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="testingContext">The testing context.</param>
        public SharedSteps(ScenarioContext scenarioContext,
                           TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
        }

        #endregion

        #region Methods

        [Given(@"I create the following api scopes")]
        public async Task GivenICreateTheFollowingApiScopes(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                CreateApiScopeRequest createApiScopeRequest = new CreateApiScopeRequest
                                                              {
                                                                  Name = SpecflowTableHelper.GetStringRowValue(tableRow, "Name"),
                                                                  Description = SpecflowTableHelper.GetStringRowValue(tableRow, "Description"),
                                                                  DisplayName = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayName")
                                                              };
                var createApiScopeResponse =
                    await this.CreateApiScope(createApiScopeRequest, CancellationToken.None).ConfigureAwait(false);

                createApiScopeResponse.ShouldNotBeNull();
                createApiScopeResponse.ApiScopeName.ShouldNotBeNullOrEmpty();
            }
        }

        private async Task<CreateApiScopeResponse> CreateApiScope(CreateApiScopeRequest createApiScopeRequest,
                                                                  CancellationToken cancellationToken)
        {
            CreateApiScopeResponse createApiScopeResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.CreateApiScope(createApiScopeRequest, cancellationToken).ConfigureAwait(false);
            return createApiScopeResponse;
        }

        /// <summary>
        /// Givens the i am logged in as with password for merchant for estate with client.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="merchantName">Name of the merchant.</param>
        /// <param name="estateName">Name of the estate.</param>
        /// <param name="clientId">The client identifier.</param>
        [Given(@"I am logged in as ""(.*)"" with password ""(.*)"" for Merchant ""(.*)"" for Estate ""(.*)"" with client ""(.*)""")]
        public async Task GivenIAmLoggedInAsWithPasswordForMerchantForEstateWithClient(String username,
                                                                                       String password,
                                                                                       String merchantName,
                                                                                       String estateName,
                                                                                       String clientId)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);

            ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

            TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                    .GetToken(username, password, clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

            estateDetails.AddMerchantUserToken(merchantName, username, tokenResponse.AccessToken);
        }

        /// <summary>
        /// Givens the i create a contract with the following values.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I create a contract with the following values")]
        public async Task GivenICreateAContractWithTheFollowingValues(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                Guid operatorId = estateDetails.GetOperatorId(operatorName);

                CreateContractRequest createContractRequest = new CreateContractRequest
                                                              {
                                                                  OperatorId = operatorId,
                                                                  Description = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription")
                                                              };

                CreateContractResponse contractResponse =
                    await this.TestingContext.DockerHelper.EstateClient.CreateContract(token, estateDetails.EstateId, createContractRequest, CancellationToken.None);

                estateDetails.AddContract(contractResponse.ContractId, createContractRequest.Description, operatorId);
            }
        }

        /// <summary>
        /// Givens the i have assigned the following devices to the merchants.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have assigned the following devices to the merchants")]
        public async Task GivenIHaveAssignedTheFollowingDevicesToTheMerchants(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                // Lookup the merchant id
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                // Lookup the operator id
                String deviceIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "DeviceIdentifier");

                AddMerchantDeviceRequest addMerchantDeviceRequest = new AddMerchantDeviceRequest
                                                                    {
                                                                        DeviceIdentifier = deviceIdentifier
                                                                    };

                AddMerchantDeviceResponse addMerchantDeviceResponse = await this.TestingContext.DockerHelper.EstateClient
                                                                                .AddDeviceToMerchant(token,
                                                                                                     estateDetails.EstateId,
                                                                                                     merchantId,
                                                                                                     addMerchantDeviceRequest,
                                                                                                     CancellationToken.None).ConfigureAwait(false);

                addMerchantDeviceResponse.EstateId.ShouldBe(estateDetails.EstateId);
                addMerchantDeviceResponse.MerchantId.ShouldBe(merchantId);
                addMerchantDeviceResponse.DeviceId.ShouldNotBe(Guid.Empty);

                this.TestingContext.Logger.LogInformation($"Device {deviceIdentifier} assigned to Merchant {merchantName} Estate {estateDetails.EstateName}");
            }
        }

        /// <summary>
        /// Givens the i have a token to access the estate management and transaction processor acl resources.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have a token to access the estate management and transaction processor acl resources")]
        public async Task GivenIHaveATokenToAccessTheEstateManagementAndTransactionProcessorAclResources(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String clientId = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientId");

                ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

                if (clientDetails.GrantType == "client_credentials")
                {
                    TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                            .GetToken(clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

                    this.TestingContext.AccessToken = tokenResponse.AccessToken;
                }
            }
        }

        /// <summary>
        /// Givens the i make the following manual merchant deposits.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I make the following manual merchant deposits")]
        public async Task GivenIMakeTheFollowingManualMerchantDeposits(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                // Lookup the merchant id
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                // Get current balance
                MerchantBalanceResponse previousMerchantBalance =
                    await this.TestingContext.DockerHelper.EstateClient.GetMerchantBalance(token, estateDetails.EstateId, merchantId, CancellationToken.None);

                MakeMerchantDepositRequest makeMerchantDepositRequest = new MakeMerchantDepositRequest
                                                                        {
                                                                            DepositDateTime =
                                                                                SpecflowTableHelper.GetDateForDateString(SpecflowTableHelper.GetStringRowValue(tableRow,
                                                                                        "DateTime"),
                                                                                    DateTime.Now),
                                                                            Reference = SpecflowTableHelper.GetStringRowValue(tableRow, "Reference"),
                                                                            Amount = SpecflowTableHelper.GetDecimalValue(tableRow, "Amount")
                                                                        };

                MakeMerchantDepositResponse makeMerchantDepositResponse = await this.TestingContext.DockerHelper.EstateClient
                                                                                    .MakeMerchantDeposit(token,
                                                                                                         estateDetails.EstateId,
                                                                                                         merchantId,
                                                                                                         makeMerchantDepositRequest,
                                                                                                         CancellationToken.None).ConfigureAwait(false);

                makeMerchantDepositResponse.EstateId.ShouldBe(estateDetails.EstateId);
                makeMerchantDepositResponse.MerchantId.ShouldBe(merchantId);
                makeMerchantDepositResponse.DepositId.ShouldNotBe(Guid.Empty);

                this.TestingContext.Logger.LogInformation($"Deposit Reference {makeMerchantDepositRequest.Reference} made for Merchant {merchantName}");

                // Check the merchant balance
                await Retry.For(async () =>
                                {
                                    MerchantBalanceResponse currentMerchantBalance =
                                        await this.TestingContext.DockerHelper.EstateClient.GetMerchantBalance(token,
                                                                                                               estateDetails.EstateId,
                                                                                                               merchantId,
                                                                                                               CancellationToken.None);

                                    currentMerchantBalance.AvailableBalance.ShouldBe(previousMerchantBalance.AvailableBalance + makeMerchantDepositRequest.Amount);
                                });
            }
        }

        /// <summary>
        /// Givens the following API resources exist.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"the following api resources exist")]
        public async Task GivenTheFollowingApiResourcesExist(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String resourceName = SpecflowTableHelper.GetStringRowValue(tableRow, "ResourceName");
                String displayName = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayName");
                String secret = SpecflowTableHelper.GetStringRowValue(tableRow, "Secret");
                String scopes = SpecflowTableHelper.GetStringRowValue(tableRow, "Scopes");
                String userClaims = SpecflowTableHelper.GetStringRowValue(tableRow, "UserClaims");

                List<String> splitScopes = scopes.Split(",").ToList();
                List<String> splitUserClaims = userClaims.Split(",").ToList();

                CreateApiResourceRequest createApiResourceRequest = new CreateApiResourceRequest
                                                                    {
                                                                        Description = string.Empty,
                                                                        DisplayName = displayName,
                                                                        Name = resourceName,
                                                                        Scopes = new List<String>(),
                                                                        Secret = secret,
                                                                        UserClaims = new List<String>()
                                                                    };
                splitScopes.ForEach(a => { createApiResourceRequest.Scopes.Add(a.Trim()); });
                splitUserClaims.ForEach(a => { createApiResourceRequest.UserClaims.Add(a.Trim()); });

                CreateApiResourceResponse createApiResourceResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                                                .CreateApiResource(createApiResourceRequest, CancellationToken.None)
                                                                                .ConfigureAwait(false);

                createApiResourceResponse.ApiResourceName.ShouldBe(resourceName);
            }
        }

        /// <summary>
        /// Givens the following clients exist.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"the following clients exist")]
        public async Task GivenTheFollowingClientsExist(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String clientId = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientId");
                String clientName = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientName");
                String secret = SpecflowTableHelper.GetStringRowValue(tableRow, "Secret");
                String allowedScopes = SpecflowTableHelper.GetStringRowValue(tableRow, "AllowedScopes");
                String allowedGrantTypes = SpecflowTableHelper.GetStringRowValue(tableRow, "AllowedGrantTypes");

                List<String> splitAllowedScopes = allowedScopes.Split(",").ToList();
                List<String> splitAllowedGrantTypes = allowedGrantTypes.Split(",").ToList();

                CreateClientRequest createClientRequest = new CreateClientRequest
                                                          {
                                                              Secret = secret,
                                                              AllowedGrantTypes = new List<String>(),
                                                              AllowedScopes = new List<String>(),
                                                              ClientDescription = string.Empty,
                                                              ClientId = clientId,
                                                              ClientName = clientName
                                                          };

                splitAllowedScopes.ForEach(a => { createClientRequest.AllowedScopes.Add(a.Trim()); });
                splitAllowedGrantTypes.ForEach(a => { createClientRequest.AllowedGrantTypes.Add(a.Trim()); });

                CreateClientResponse createClientResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                                      .CreateClient(createClientRequest, CancellationToken.None).ConfigureAwait(false);

                createClientResponse.ClientId.ShouldBe(clientId);

                this.TestingContext.AddClientDetails(clientId, secret, allowedGrantTypes);
            }
        }

        /// <summary>
        /// Givens the following security roles exist.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"the following security roles exist")]
        public async Task GivenTheFollowingSecurityRolesExist(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String roleName = SpecflowTableHelper.GetStringRowValue(tableRow, "RoleName");

                CreateRoleRequest createRoleRequest = new CreateRoleRequest
                                                      {
                                                          RoleName = roleName
                                                      };

                CreateRoleResponse createRoleResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.CreateRole(createRoleRequest, CancellationToken.None)
                                                                  .ConfigureAwait(false);

                createRoleResponse.RoleId.ShouldNotBe(Guid.Empty);
            }
        }

        /// <summary>
        /// Thens the reconciliation response should contain the following information.
        /// </summary>
        /// <param name="table">The table.</param>
        [Then(@"the reconciliation response should contain the following information")]
        public void ThenReconciliationResponseShouldContainTheFollowingInformation(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                // Get the merchant name
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);
                String responseMessage = estateDetails.GetReconciliationResponse(merchantId);

                ReconciliationResponseMessage transactionResponse = JsonConvert.DeserializeObject<ReconciliationResponseMessage>(responseMessage);
                this.ValidateTransactionResponse((dynamic)transactionResponse, tableRow);
            }
        }

        /// <summary>
        /// Thens the transaction response should contain the following information.
        /// </summary>
        /// <param name="table">The table.</param>
        [Then(@"the logon transaction response should contain the following information")]
        public void ThenTheLogonTransactionResponseShouldContainTheFollowingInformation(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String responseMessage = this.GetResponseMessage(tableRow);

                LogonTransactionResponseMessage transactionResponse = JsonConvert.DeserializeObject<LogonTransactionResponseMessage>(responseMessage);
                this.ValidateTransactionResponse((dynamic)transactionResponse, tableRow);
            }
        }

        [Then(@"the sale transaction response should contain the following information")]
        public void ThenTheSaleTransactionResponseShouldContainTheFollowingInformation(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String responseMessage = this.GetResponseMessage(tableRow);

                SaleTransactionResponseMessage transactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponseMessage>(responseMessage);
                this.ValidateTransactionResponse((dynamic)transactionResponse, tableRow);
            }
        }

        private String GetResponseMessage(TableRow tableRow )
        {
            // Get the merchant name
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

            String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
            Guid merchantId = estateDetails.GetMerchantId(merchantName);

            String transactionNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionNumber");
            String transactionType = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionType");
            String responseMessage = estateDetails.GetTransactionResponse(merchantId, transactionNumber, transactionType);

            return responseMessage;
        }

        /// <summary>
        /// Whens the i add the following transaction fees.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I add the following Transaction Fees")]
        public async Task WhenIAddTheFollowingTransactionFees(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String contractName = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription");
                String productName = SpecflowTableHelper.GetStringRowValue(tableRow, "ProductName");
                Contract contract = estateDetails.GetContract(contractName);

                Product product = contract.GetProduct(productName);

                AddTransactionFeeForProductToContractRequest addTransactionFeeForProductToContractRequest = new AddTransactionFeeForProductToContractRequest
                                                                                                            {
                                                                                                                Value =
                                                                                                                    SpecflowTableHelper
                                                                                                                        .GetDecimalValue(tableRow, "Value"),
                                                                                                                Description =
                                                                                                                    SpecflowTableHelper.GetStringRowValue(tableRow,
                                                                                                                        "FeeDescription"),
                                                                                                                CalculationType =
                                                                                                                    SpecflowTableHelper
                                                                                                                        .GetEnumValue<CalculationType>(tableRow,
                                                                                                                            "CalculationType")
                                                                                                            };

                AddTransactionFeeForProductToContractResponse addTransactionFeeForProductToContractResponse =
                    await this.TestingContext.DockerHelper.EstateClient.AddTransactionFeeForProductToContract(token,
                                                                                                              estateDetails.EstateId,
                                                                                                              contract.ContractId,
                                                                                                              product.ProductId,
                                                                                                              addTransactionFeeForProductToContractRequest,
                                                                                                              CancellationToken.None);

                product.AddTransactionFee(addTransactionFeeForProductToContractResponse.TransactionFeeId,
                                          addTransactionFeeForProductToContractRequest.CalculationType,
                                          addTransactionFeeForProductToContractRequest.Description,
                                          addTransactionFeeForProductToContractRequest.Value);
            }
        }

        /// <summary>
        /// Whens the i assign the following operator to the merchants.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have assigned the following  operator to the merchants")]
        [When(@"I assign the following  operator to the merchants")]
        public async Task WhenIAssignTheFollowingOperatorToTheMerchants(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                // Lookup the merchant id
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                // Lookup the operator id
                String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                Guid operatorId = estateDetails.GetOperatorId(operatorName);

                AssignOperatorRequest assignOperatorRequest = new AssignOperatorRequest
                                                              {
                                                                  OperatorId = operatorId,
                                                                  MerchantNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantNumber"),
                                                                  TerminalNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TerminalNumber"),
                                                              };

                AssignOperatorResponse assignOperatorResponse = await this.TestingContext.DockerHelper.EstateClient
                                                                          .AssignOperatorToMerchant(token,
                                                                                                    estateDetails.EstateId,
                                                                                                    merchantId,
                                                                                                    assignOperatorRequest,
                                                                                                    CancellationToken.None).ConfigureAwait(false);

                assignOperatorResponse.EstateId.ShouldBe(estateDetails.EstateId);
                assignOperatorResponse.MerchantId.ShouldBe(merchantId);
                assignOperatorResponse.OperatorId.ShouldBe(operatorId);

                this.TestingContext.Logger.LogInformation($"Operator {operatorName} assigned to Estate {estateDetails.EstateName}");
            }
        }

        /// <summary>
        /// Whens the i create the following estates.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have created the following estates")]
        [When(@"I create the following estates")]
        public async Task WhenICreateTheFollowingEstates(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String estateName = SpecflowTableHelper.GetStringRowValue(tableRow, "EstateName");

                CreateEstateRequest createEstateRequest = new CreateEstateRequest
                                                          {
                                                              EstateId = Guid.NewGuid(),
                                                              EstateName = estateName
                                                          };

                CreateEstateResponse response = await this.TestingContext.DockerHelper.EstateClient
                                                          .CreateEstate(this.TestingContext.AccessToken, createEstateRequest, CancellationToken.None)
                                                          .ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.EstateId.ShouldNotBe(Guid.Empty);

                // Cache the estate id
                this.TestingContext.AddEstateDetails(response.EstateId, estateName);

                this.TestingContext.Logger.LogInformation($"Estate {estateName} created with Id {response.EstateId}");
            
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                // Setup the subscriptions for the estate
                await Retry.For(async () =>
                                {
                                    await this.TestingContext.DockerHelper.PopulateSubscriptionServiceConfiguration(estateName,this.TestingContext.DockerHelper.IsSecureEventStore).ConfigureAwait(false);
                                }, retryFor: TimeSpan.FromMinutes(2), retryInterval: TimeSpan.FromSeconds(30));

                EstateResponse estate = null;
                await Retry.For(async () =>
                                {
                                    estate = await this.TestingContext.DockerHelper.EstateClient
                                                       .GetEstate(this.TestingContext.AccessToken, estateDetails.EstateId, CancellationToken.None).ConfigureAwait(false);
                                    estate.ShouldNotBeNull();
                                }, TimeSpan.FromMinutes(2)).ConfigureAwait(false);

                estate.EstateName.ShouldBe(estateDetails.EstateName);
            }
        }

        /// <summary>
        /// Whens the i create the following merchants.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given("I create the following merchants")]
        [When(@"I create the following merchants")]
        public async Task WhenICreateTheFollowingMerchants(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                // lookup the estate id based on the name in the table
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);
                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                CreateMerchantRequest createMerchantRequest = new CreateMerchantRequest
                                                              {
                                                                  Name = merchantName,
                                                                  Contact = new Contact
                                                                            {
                                                                                ContactName = SpecflowTableHelper.GetStringRowValue(tableRow, "ContactName"),
                                                                                EmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "EmailAddress")
                                                                            },
                                                                  Address = new Address
                                                                            {
                                                                                AddressLine1 = SpecflowTableHelper.GetStringRowValue(tableRow, "AddressLine1"),
                                                                                Town = SpecflowTableHelper.GetStringRowValue(tableRow, "Town"),
                                                                                Region = SpecflowTableHelper.GetStringRowValue(tableRow, "Region"),
                                                                                Country = SpecflowTableHelper.GetStringRowValue(tableRow, "Country")
                                                                            }
                                                              };

                CreateMerchantResponse response = await this.TestingContext.DockerHelper.EstateClient
                                                            .CreateMerchant(token, estateDetails.EstateId, createMerchantRequest, CancellationToken.None)
                                                            .ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.EstateId.ShouldBe(estateDetails.EstateId);
                response.MerchantId.ShouldNotBe(Guid.Empty);

                // Cache the merchant id
                estateDetails.AddMerchant(response.MerchantId, merchantName);

                this.TestingContext.Logger.LogInformation($"Merchant {merchantName} created with Id {response.MerchantId} for Estate {estateDetails.EstateName}");
            }

            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");

                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                await Retry.For(async () =>
                                {
                                    MerchantResponse merchant = await this.TestingContext.DockerHelper.EstateClient
                                                                          .GetMerchant(token, estateDetails.EstateId, merchantId, CancellationToken.None)
                                                                          .ConfigureAwait(false);

                                    merchant.MerchantName.ShouldBe(merchantName);
                                });
            }
        }

        /// <summary>
        /// Whens the i create the following operators.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have created the following operators")]
        [When(@"I create the following operators")]
        public async Task WhenICreateTheFollowingOperators(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                Boolean requireCustomMerchantNumber = SpecflowTableHelper.GetBooleanValue(tableRow, "RequireCustomMerchantNumber");
                Boolean requireCustomTerminalNumber = SpecflowTableHelper.GetBooleanValue(tableRow, "RequireCustomTerminalNumber");

                CreateOperatorRequest createOperatorRequest = new CreateOperatorRequest
                                                              {
                                                                  Name = operatorName,
                                                                  RequireCustomMerchantNumber = requireCustomMerchantNumber,
                                                                  RequireCustomTerminalNumber = requireCustomTerminalNumber
                                                              };

                // lookup the estate id based on the name in the table
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                CreateOperatorResponse response = await this.TestingContext.DockerHelper.EstateClient
                                                            .CreateOperator(this.TestingContext.AccessToken,
                                                                            estateDetails.EstateId,
                                                                            createOperatorRequest,
                                                                            CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.EstateId.ShouldNotBe(Guid.Empty);
                response.OperatorId.ShouldNotBe(Guid.Empty);

                // Cache the estate id
                estateDetails.AddOperator(response.OperatorId, operatorName);

                this.TestingContext.Logger.LogInformation($"Operator {operatorName} created with Id {response.OperatorId} for Estate {estateDetails.EstateName}");
            }
        }

        /// <summary>
        /// Whens the i create the following products.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I create the following Products")]
        public async Task WhenICreateTheFollowingProducts(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String contractName = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription");
                Contract contract = estateDetails.GetContract(contractName);
                String productValue = SpecflowTableHelper.GetStringRowValue(tableRow, "Value");

                AddProductToContractRequest addProductToContractRequest = new AddProductToContractRequest
                                                                          {
                                                                              ProductName = SpecflowTableHelper.GetStringRowValue(tableRow, "ProductName"),
                                                                              DisplayText = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayText"),
                                                                              Value = null
                                                                          };
                if (string.IsNullOrEmpty(productValue) == false)
                {
                    addProductToContractRequest.Value = decimal.Parse(productValue);
                }

                AddProductToContractResponse addProductToContractResponse =
                    await this.TestingContext.DockerHelper.EstateClient.AddProductToContract(token,
                                                                                             estateDetails.EstateId,
                                                                                             contract.ContractId,
                                                                                             addProductToContractRequest,
                                                                                             CancellationToken.None);

                contract.AddProduct(addProductToContractResponse.ProductId,
                                    addProductToContractRequest.ProductName,
                                    addProductToContractRequest.DisplayText,
                                    addProductToContractRequest.Value);
            }
        }

        /// <summary>
        /// Whens the i create the following security users.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I create the following security users")]
        [Given("I have created the following security users")]
        public async Task WhenICreateTheFollowingSecurityUsers(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                // lookup the estate id based on the name in the table
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                if (tableRow.ContainsKey("EstateName") && tableRow.ContainsKey("MerchantName") == false)
                {
                    // Creating an Estate User
                    CreateEstateUserRequest createEstateUserRequest = new CreateEstateUserRequest
                                                                      {
                                                                          EmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "EmailAddress"),
                                                                          FamilyName = SpecflowTableHelper.GetStringRowValue(tableRow, "FamilyName"),
                                                                          GivenName = SpecflowTableHelper.GetStringRowValue(tableRow, "GivenName"),
                                                                          MiddleName = SpecflowTableHelper.GetStringRowValue(tableRow, "MiddleName"),
                                                                          Password = SpecflowTableHelper.GetStringRowValue(tableRow, "Password")
                                                                      };

                    CreateEstateUserResponse createEstateUserResponse =
                        await this.TestingContext.DockerHelper.EstateClient.CreateEstateUser(this.TestingContext.AccessToken,
                                                                                             estateDetails.EstateId,
                                                                                             createEstateUserRequest,
                                                                                             CancellationToken.None);

                    createEstateUserResponse.EstateId.ShouldBe(estateDetails.EstateId);
                    createEstateUserResponse.UserId.ShouldNotBe(Guid.Empty);

                    estateDetails.SetEstateUser(createEstateUserRequest.EmailAddress, createEstateUserRequest.Password);

                    this.TestingContext.Logger.LogInformation($"Security user {createEstateUserRequest.EmailAddress} assigned to Estate {estateDetails.EstateName}");
                }
                else if (tableRow.ContainsKey("MerchantName"))
                {
                    // Creating a merchant user
                    String token = this.TestingContext.AccessToken;
                    if (string.IsNullOrEmpty(estateDetails.AccessToken) == false)
                    {
                        token = estateDetails.AccessToken;
                    }

                    // lookup the merchant id based on the name in the table
                    String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                    Guid merchantId = estateDetails.GetMerchantId(merchantName);

                    CreateMerchantUserRequest createMerchantUserRequest = new CreateMerchantUserRequest
                                                                          {
                                                                              EmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "EmailAddress"),
                                                                              FamilyName = SpecflowTableHelper.GetStringRowValue(tableRow, "FamilyName"),
                                                                              GivenName = SpecflowTableHelper.GetStringRowValue(tableRow, "GivenName"),
                                                                              MiddleName = SpecflowTableHelper.GetStringRowValue(tableRow, "MiddleName"),
                                                                              Password = SpecflowTableHelper.GetStringRowValue(tableRow, "Password")
                                                                          };

                    CreateMerchantUserResponse createMerchantUserResponse =
                        await this.TestingContext.DockerHelper.EstateClient.CreateMerchantUser(token,
                                                                                               estateDetails.EstateId,
                                                                                               merchantId,
                                                                                               createMerchantUserRequest,
                                                                                               CancellationToken.None);

                    createMerchantUserResponse.EstateId.ShouldBe(estateDetails.EstateId);
                    createMerchantUserResponse.MerchantId.ShouldBe(merchantId);
                    createMerchantUserResponse.UserId.ShouldNotBe(Guid.Empty);

                    estateDetails.AddMerchantUser(merchantName, createMerchantUserRequest.EmailAddress, createMerchantUserRequest.Password);

                    this.TestingContext.Logger.LogInformation($"Security user {createMerchantUserRequest.EmailAddress} assigned to Merchant {merchantName}");
                }
            }
        }

        /// <summary>
        /// Whens the i perform the following reconciliations.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I perform the following reconciliations")]
        public async Task WhenIPerformTheFollowingReconciliations(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                String dateString = SpecflowTableHelper.GetStringRowValue(tableRow, "DateTime");
                DateTime transactionDateTime = SpecflowTableHelper.GetDateForDateString(dateString, DateTime.Today);
                String deviceIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "DeviceIdentifier");
                Int32 transactionCount = SpecflowTableHelper.GetIntValue(tableRow, "TransactionCount");
                Decimal transactionValue = SpecflowTableHelper.GetDecimalValue(tableRow, "TransactionValue");

                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                // Lookup the merchant id
                Guid merchantId = estateDetails.GetMerchantId(merchantName);
                String merchantToken = estateDetails.GetMerchantUserToken(merchantName);

                String reconciliationResponse = await this.PerformReconciliationTransaction(merchantToken,
                                                                                            transactionDateTime,
                                                                                            deviceIdentifier,
                                                                                            transactionCount,
                                                                                            transactionValue,
                                                                                            CancellationToken.None);

                estateDetails.AddReconciliationResponse(merchantId, reconciliationResponse);
            }
        }

        /// <summary>
        /// Whens the i perform the following transactions.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I perform the following transactions")]
        public async Task WhenIPerformTheFollowingTransactions(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);
                String merchantToken = estateDetails.GetMerchantUserToken(merchantName);

                String dateString = SpecflowTableHelper.GetStringRowValue(tableRow, "DateTime");
                DateTime transactionDateTime = SpecflowTableHelper.GetDateForDateString(dateString, DateTime.Today);
                String transactionNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionNumber");
                String transactionType = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionType");
                String deviceIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "DeviceIdentifier");

                String responseMessage = null;
                switch(transactionType)
                {
                    case "Logon":
                        responseMessage = await this.PerformLogonTransaction(merchantToken,
                                                                             transactionDateTime,
                                                                             transactionType,
                                                                             transactionNumber,
                                                                             deviceIdentifier,
                                                                             CancellationToken.None);

                        break;
                    case "Sale":
                        String operatorIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                        Decimal transactionAmount = SpecflowTableHelper.GetDecimalValue(tableRow, "TransactionAmount");
                        String customerAccountNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "CustomerAccountNumber");
                        String customerEmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "CustomerEmailAddress");
                        String contractDescription = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription");
                        String productName = SpecflowTableHelper.GetStringRowValue(tableRow, "ProductName");
                        String recipientEmail = SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientEmail");
                        String recipientMobile = SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile");

                        Guid contractId = Guid.Empty;
                        Guid productId = Guid.Empty;
                        var contract = estateDetails.GetContract(contractDescription);
                        if (contract != null)
                        {
                            contractId = contract.ContractId;
                            var product = contract.GetProduct(productName);
                            productId = product.ProductId;
                        }

                        Dictionary<String, String> additionalRequestMetaData = new Dictionary<String, String>();

                        if (transactionAmount > 0)
                        {
                            additionalRequestMetaData.Add("Amount", transactionAmount.ToString());
                        }

                        if (string.IsNullOrEmpty(customerAccountNumber) == false)
                        {
                            additionalRequestMetaData.Add("CustomerAccountNumber", customerAccountNumber);
                        }

                        if (string.IsNullOrEmpty(recipientEmail) == false)
                        {
                            additionalRequestMetaData.Add("RecipientEmail", recipientEmail);
                        }

                        if (string.IsNullOrEmpty(recipientMobile) == false)
                        {
                            additionalRequestMetaData.Add("RecipientMobile", recipientMobile);
                        }

                        responseMessage = await this.PerformSaleTransaction(merchantToken,
                                                                            transactionDateTime,
                                                                            transactionType,
                                                                            transactionNumber,
                                                                            deviceIdentifier,
                                                                            operatorIdentifier,
                                                                            customerEmailAddress,
                                                                            contractId,
                                                                            productId,
                                                                            additionalRequestMetaData,
                                                                            CancellationToken.None);
                        break;
                }

                responseMessage.ShouldNotBeNullOrEmpty("No response message received");

                estateDetails.AddTransactionResponse(merchantId, transactionNumber, transactionType, responseMessage);
            }
        }

        /// <summary>
        /// Performs the logon transaction.
        /// </summary>
        /// <param name="merchantToken">The merchant token.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<String> PerformLogonTransaction(String merchantToken,
                                                           DateTime transactionDateTime,
                                                           String transactionType,
                                                           String transactionNumber,
                                                           String deviceIdentifier,
                                                           CancellationToken cancellationToken)
        {
            LogonTransactionRequestMessage logonTransactionRequestMessage = new LogonTransactionRequestMessage
                                                                            {
                                                                                DeviceIdentifier = deviceIdentifier,
                                                                                TransactionDateTime = transactionDateTime,
                                                                                TransactionNumber = transactionNumber,
                                                                                ApplicationVersion = SharedSteps.ApplicationVersion
                                                                            };

            String uri = "api/transactions";

            StringContent content = new StringContent(JsonConvert.SerializeObject(logonTransactionRequestMessage,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  }),
                                                      Encoding.UTF8,
                                                      "application/json");

            this.TestingContext.DockerHelper.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", merchantToken);

            HttpResponseMessage response = await this.TestingContext.DockerHelper.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

            response.IsSuccessStatusCode.ShouldBeTrue();

            String responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return responseContent;
        }

        /// <summary>
        /// Performs the reconciliation transaction.
        /// </summary>
        /// <param name="merchantToken">The merchant token.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="transactionCount">The transaction count.</param>
        /// <param name="transactionValue">The transaction value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<String> PerformReconciliationTransaction(String merchantToken,
                                                                    DateTime transactionDateTime,
                                                                    String deviceIdentifier,
                                                                    Int32 transactionCount,
                                                                    Decimal transactionValue,
                                                                    CancellationToken cancellationToken)
        {
            ReconciliationRequestMessage reconciliationRequestMessage = new ReconciliationRequestMessage
                                                                        {
                                                                            TransactionDateTime = transactionDateTime,
                                                                            DeviceIdentifier = deviceIdentifier,
                                                                            TransactionValue = transactionValue,
                                                                            TransactionCount = transactionCount,
                                                                            ApplicationVersion = SharedSteps.ApplicationVersion
                                                                        };

            String uri = "api/transactions";

            StringContent content = new StringContent(JsonConvert.SerializeObject(reconciliationRequestMessage,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  }),
                                                      Encoding.UTF8,
                                                      "application/json");

            this.TestingContext.DockerHelper.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", merchantToken);

            HttpResponseMessage response = await this.TestingContext.DockerHelper.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

            response.IsSuccessStatusCode.ShouldBeTrue();

            String responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return responseContent;
        }

        /// <summary>
        /// Performs the sale transaction.
        /// </summary>
        /// <param name="merchantToken">The merchant token.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="operatorIdentifier">The operator identifier.</param>
        /// <param name="customerEmailAddress">The customer email address.</param>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="additionalRequestMetaData">The additional request meta data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<String> PerformSaleTransaction(String merchantToken,
                                                          DateTime transactionDateTime,
                                                          String transactionType,
                                                          String transactionNumber,
                                                          String deviceIdentifier,
                                                          String operatorIdentifier,
                                                          String customerEmailAddress,
                                                          Guid contractId,
                                                          Guid productId,
                                                          Dictionary<String, String> additionalRequestMetaData,
                                                          CancellationToken cancellationToken)
        {
            SaleTransactionRequestMessage saleTransactionRequestMessage = new SaleTransactionRequestMessage
                                                                          {
                                                                              DeviceIdentifier = deviceIdentifier,
                                                                              TransactionDateTime = transactionDateTime,
                                                                              TransactionNumber = transactionNumber,
                                                                              OperatorIdentifier = operatorIdentifier,
                                                                              CustomerEmailAddress = customerEmailAddress,
                                                                              ContractId = contractId,
                                                                              ProductId = productId,
                                                                              ApplicationVersion = SharedSteps.ApplicationVersion
                                                                          };

            saleTransactionRequestMessage.AdditionalRequestMetaData = additionalRequestMetaData;

            String uri = "api/transactions";

            StringContent content = new StringContent(JsonConvert.SerializeObject(saleTransactionRequestMessage,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  }),
                                                      Encoding.UTF8,
                                                      "application/json");

            this.TestingContext.DockerHelper.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", merchantToken);

            HttpResponseMessage response = await this.TestingContext.DockerHelper.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

            response.IsSuccessStatusCode.ShouldBeTrue();

            String responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return responseContent;
        }

        /// <summary>
        /// Validates the transaction response.
        /// </summary>
        /// <param name="reconciliationResponseMessage">The reconciliation response message.</param>
        /// <param name="tableRow">The table row.</param>
        private void ValidateTransactionResponse(ReconciliationResponseMessage reconciliationResponseMessage,
                                                 TableRow tableRow)
        {
            String expectedResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode");
            String expectedResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage");

            reconciliationResponseMessage.ResponseCode.ShouldBe(expectedResponseCode);
            reconciliationResponseMessage.ResponseMessage.ShouldBe(expectedResponseMessage);
        }

        /// <summary>
        /// Validates the transaction response.
        /// </summary>
        /// <param name="logonTransactionResponseMessage">The logon transaction response message.</param>
        /// <param name="tableRow">The table row.</param>
        private void ValidateTransactionResponse(LogonTransactionResponseMessage logonTransactionResponseMessage,
                                                 TableRow tableRow)
        {
            String expectedResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode");
            String expectedResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage");

            logonTransactionResponseMessage.ResponseCode.ShouldBe(expectedResponseCode);
            logonTransactionResponseMessage.ResponseMessage.ShouldBe(expectedResponseMessage);
        }

        /// <summary>
        /// Validates the transaction response.
        /// </summary>
        /// <param name="saleTransactionResponseMessage">The sale transaction response message.</param>
        /// <param name="tableRow">The table row.</param>
        private void ValidateTransactionResponse(SaleTransactionResponseMessage saleTransactionResponseMessage,
                                                 TableRow tableRow)
        {
            String expectedResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode");
            String expectedResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage");

            saleTransactionResponseMessage.ResponseCode.ShouldBe(expectedResponseCode);
            saleTransactionResponseMessage.ResponseMessage.ShouldBe(expectedResponseMessage);
        }

        #endregion

        #region Others

        /// <summary>
        /// The application version
        /// </summary>
        private const String ApplicationVersion = "1.0.5";

        #endregion
    }
}