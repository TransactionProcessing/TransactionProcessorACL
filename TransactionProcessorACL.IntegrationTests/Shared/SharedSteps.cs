using TransactionProcessor.DataTransferObjects.Requests.Contract;
using TransactionProcessor.DataTransferObjects.Requests.Estate;
using TransactionProcessor.DataTransferObjects.Requests.Merchant;
using TransactionProcessor.DataTransferObjects.Requests.Operator;
using TransactionProcessor.DataTransferObjects.Responses.Contract;
using TransactionProcessor.DataTransferObjects.Responses.Estate;
using TransactionProcessor.DataTransferObjects.Responses.Merchant;
using AssignOperatorRequest = TransactionProcessor.DataTransferObjects.Requests.Merchant.AssignOperatorRequest;

namespace TransactionProcessorACL.IntegrationTests.Shared{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using DataTransferObjects.Responses;
    using Newtonsoft.Json.Linq;
    using Reqnroll;
    using SecurityService.DataTransferObjects;
    using SecurityService.DataTransferObjects.Requests;
    using SecurityService.DataTransferObjects.Responses;
    using SecurityService.IntegrationTesting.Helpers;
    using Shouldly;
    using TransactionProcessor.DataTransferObjects;
    using TransactionProcessor.IntegrationTesting.Helpers;
    using TransactionProcessor.IntegrationTests.Common;
    using ClientDetails = TransactionProcessor.IntegrationTests.Common.ClientDetails;
    
    /// <summary>
    /// 
    /// </summary>
    [Binding]
    [Scope(Tag = "shared")]
    public class SharedSteps{
        #region Fields

        private readonly ACLSteps AclSteps;

        private readonly TransactionProcessorSteps TransactionProcessorSteps;

        /// <summary>
        /// The scenario context
        /// </summary>
        private readonly ScenarioContext ScenarioContext;

        private readonly SecurityServiceSteps SecurityServiceSteps;

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
                           TestingContext testingContext){
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
            this.SecurityServiceSteps = new SecurityServiceSteps(testingContext.DockerHelper.SecurityServiceClient);
            this.TransactionProcessorSteps = new TransactionProcessorSteps(testingContext.DockerHelper.TransactionProcessorClient, testingContext.DockerHelper.TestHostHttpClient, testingContext.DockerHelper.ProjectionManagementClient);
            this.AclSteps = new ACLSteps(testingContext.DockerHelper.HttpClient, this.TestingContext.DockerHelper.TransactionProcessorClient);
        }

        #endregion

        #region Methods

        [Given(@"I am logged in as ""([^""]*)"" with password ""([^""]*)"" for Estate ""([^""]*)"" with client ""([^""]*)""")]
        public async Task GivenIAmLoggedInAsWithPasswordForEstateWithClient(String username, String password, String estateName, String clientId){
            EstateDetails1 estateDetails = this.TestingContext.GetEstateDetails(estateName);
            ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

            TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                    .GetToken(username, password, clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

            estateDetails.AddVoucherRedemptionUserToken("Voucher", username, tokenResponse.AccessToken);
        }

        [Given(@"I am logged in as ""([^""]*)"" with password ""([^""]*)"" for Merchant ""([^""]*)"" for Estate ""([^""]*)"" with client ""([^""]*)""")]
        public async Task GivenIAmLoggedInAsWithPasswordForMerchantForEstateWithClient(String username,
                                                                                       String password,
                                                                                       String merchantName,
                                                                                       String estateName,
                                                                                       String clientId){
            EstateDetails1 estateDetails = this.TestingContext.GetEstateDetails(estateName);
            Guid merchantId = estateDetails.EstateDetails.GetMerchantId(merchantName);
            ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

            TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                    .GetToken(username, password, clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

            estateDetails.AddMerchantUserToken(merchantId, username, tokenResponse.AccessToken);
        }
        /// <summary>
        /// Givens the i am logged in as with password for merchant for estate with client.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="merchantName">Name of the merchant.</param>
        /// <param name="estateName">Name of the estate.</param>
        /// <param name="clientId">The client identifier.</param>
        //[Given(@"I am logged in as ""(.*)"" with password ""(.*)"" for Merchant ""(.*)"" for Estate ""(.*)"" with client ""(.*)""")]
        //public async Task GivenIAmLoggedInAsWithPasswordForMerchantForEstateWithClient(String username,
        //                                                                               String password,
        //                                                                               String merchantName,
        //                                                                               String estateName,
        //                                                                               String clientId)
        //{
        //    EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);
        //    ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

        //    TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
        //                                            .GetToken(username, password, clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

        //    estateDetails.AddMerchantUserToken(merchantName, username, tokenResponse.AccessToken);
        //}

        /// <summary>
        /// Givens the i create a contract with the following values.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I create a contract with the following values")]
        public async Task GivenICreateAContractWithTheFollowingValues(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, CreateContractRequest)> requests = table.Rows.ToCreateContractRequests(estates);
            List<ContractResponse> responses = await this.TransactionProcessorSteps.GivenICreateAContractWithTheFollowingValues(this.TestingContext.AccessToken, requests);
            foreach (ContractResponse contractResponse in responses)
            {
                var estate = this.TestingContext.Estates.Single(e => e.EstateDetails.EstateId == contractResponse.EstateId);
                estate.EstateDetails.AddContract(contractResponse.ContractId, contractResponse.Description, contractResponse.OperatorId);
            }
        }

        [Given(@"I create the following api scopes")]
        public async Task GivenICreateTheFollowingApiScopes(DataTable table){
            List<CreateApiScopeRequest> requests = table.Rows.ToCreateApiScopeRequests();
            await this.SecurityServiceSteps.GivenICreateTheFollowingApiScopes(requests);
        }

        /// <summary>
        /// Givens the i have assigned the following devices to the merchants.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have assigned the following devices to the merchants")]
        public async Task GivenIHaveAssignedTheFollowingDevicesToTheMerchants(DataTable table){
            List<EstateDetails> estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, Guid, AddMerchantDeviceRequest)> requests = table.Rows.ToAddMerchantDeviceRequests(estates);

            List<(EstateDetails, MerchantResponse, String)> results = await this.TransactionProcessorSteps.GivenIHaveAssignedTheFollowingDevicesToTheMerchants(this.TestingContext.AccessToken, requests);
            foreach ((EstateDetails, MerchantResponse, String) result in results){
                this.TestingContext.Logger.LogInformation($"Device {result.Item3} assigned to Merchant {result.Item2.MerchantName} Estate {result.Item1.EstateName}");
            }
        }

        [When(@"I add the following contracts to the following merchants")]
        public async Task WhenIAddTheFollowingContractsToTheFollowingMerchants(DataTable table){
            List<EstateDetails> estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, Guid, Guid)> requests = table.Rows.ToAddContractToMerchantRequests(estates);
            await this.TransactionProcessorSteps.WhenIAddTheFollowingContractsToTheFollowingMerchants(this.TestingContext.AccessToken, requests);
        }


        /// <summary>
        /// Givens the i have a token to access the estate management and transaction processor acl resources.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have a token to access the estate management and transaction processor acl resources")]
        public async Task GivenIHaveATokenToAccessTheEstateManagementAndTransactionProcessorAclResources(DataTable table){
            DataTableRow firstRow = table.Rows.First();
            String clientId = ReqnrollTableHelper.GetStringRowValue(firstRow, "ClientId");
            ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

            this.TestingContext.AccessToken = await this.SecurityServiceSteps.GetClientToken(clientDetails.ClientId, clientDetails.ClientSecret, CancellationToken.None);
        }

        private async Task<Decimal> GetMerchantBalance(Guid merchantId)
        {
            JsonElement jsonElement = (JsonElement)await this.TestingContext.DockerHelper.ProjectionManagementClient.GetStateAsync<dynamic>("MerchantBalanceProjection", $"MerchantBalance-{merchantId:N}");
            JObject jsonObject = JObject.Parse(jsonElement.GetRawText());
            decimal balanceValue = jsonObject.SelectToken("merchant.balance").Value<decimal>();
            return balanceValue;
        }

        /// <summary>
        /// Givens the i make the following manual merchant deposits.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I make the following manual merchant deposits")]
        public async Task GivenIMakeTheFollowingManualMerchantDeposits(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, Guid, MakeMerchantDepositRequest)> requests = table.Rows.ToMakeMerchantDepositRequest(estates);

            foreach ((EstateDetails, Guid, MakeMerchantDepositRequest) request in requests){
                Decimal previousMerchantBalance = await this.GetMerchantBalance(request.Item2);

                await this.TransactionProcessorSteps.GivenIMakeTheFollowingManualMerchantDeposits(this.TestingContext.AccessToken, request);

                await Retry.For(async () => {
                    Decimal currentMerchantBalance = await this.GetMerchantBalance(request.Item2);

                    currentMerchantBalance.ShouldBe(previousMerchantBalance + request.Item3.Amount);

                    this.TestingContext.Logger.LogInformation($"Deposit Reference {request.Item3.Reference} made for Merchant Id {request.Item2}");
                                });
            }
        }

        [Given(@"the following api resources exist")]
        public async Task GivenTheFollowingApiResourcesExist(DataTable table){
            List<CreateApiResourceRequest> requests = table.Rows.ToCreateApiResourceRequests();
            await this.SecurityServiceSteps.GivenTheFollowingApiResourcesExist(requests);
        }

        /// <summary>
        /// Givens the following clients exist.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"the following clients exist")]
        public async Task GivenTheFollowingClientsExist(DataTable table){
            List<CreateClientRequest> requests = table.Rows.ToCreateClientRequests();
            List<(String clientId, String secret, List<String> allowedGrantTypes)> results = await this.SecurityServiceSteps.GivenTheFollowingClientsExist(requests);

            foreach ((String clientId, String secret, List<String> allowedGrantTypes) result in results){
                this.TestingContext.AddClientDetails(result.clientId, result.secret, String.Join(",", result.allowedGrantTypes));
            }
        }

        /// <summary>
        /// Givens the following security roles exist.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"the following security roles exist")]
        public async Task GivenTheFollowingSecurityRolesExist(DataTable table){
            List<CreateRoleRequest> requests = table.Rows.ToCreateRoleRequests();
            await this.SecurityServiceSteps.GivenICreateTheFollowingRoles(requests, CancellationToken.None);
        }

        /// <summary>
        /// Thens the reconciliation response should contain the following information.
        /// </summary>
        /// <param name="table">The table.</param>
        [Then(@"the reconciliation response should contain the following information")]
        public void ThenReconciliationResponseShouldContainTheFollowingInformation(DataTable table){
            List<ReqnrollExtensions.ExpectedReconciliationResponse> expectedResponses = table.Rows.ToExpectedReconciliationResponseDetails(this.TestingContext.Estates);
            this.AclSteps.ThenReconciliationResponseShouldContainTheFollowingInformation(expectedResponses, this.TestingContext.Estates);
        }

        /// <summary>
        /// Thens the transaction response should contain the following information.
        /// </summary>
        /// <param name="table">The table.</param>
        [Then(@"the logon transaction response should contain the following information")]
        public void ThenTheLogonTransactionResponseShouldContainTheFollowingInformation(DataTable table){
            List<ReqnrollExtensions.ExpectedTransactionResponse> expectedResponses = table.Rows.ToExpectedTransactionResponseDetails(this.TestingContext.Estates);
            this.AclSteps.ThenTheLogonTransactionResponseShouldContainTheFollowingInformation(expectedResponses, this.TestingContext.Estates);
        }

        [Then(@"the sale transaction response should contain the following information")]
        public void ThenTheSaleTransactionResponseShouldContainTheFollowingInformation(DataTable table){
            List<ReqnrollExtensions.ExpectedTransactionResponse> expectedResponses = table.Rows.ToExpectedTransactionResponseDetails(this.TestingContext.Estates);
            this.AclSteps.ThenTheSaleTransactionResponseShouldContainTheFollowingInformation(expectedResponses, this.TestingContext.Estates);
        }

        /// <summary>
        /// Whens the i add the following transaction fees.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I add the following Transaction Fees")]
        public async Task WhenIAddTheFollowingTransactionFees(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, TransactionProcessor.IntegrationTesting.Helpers.Contract, TransactionProcessor.IntegrationTesting.Helpers.Product, AddTransactionFeeForProductToContractRequest)> requests = table.Rows.ToAddTransactionFeeForProductToContractRequests(estates);
            await this.TransactionProcessorSteps.WhenIAddTheFollowingTransactionFees(this.TestingContext.AccessToken, requests);
        }

        /// <summary>
        /// Whens the i assign the following operator to the merchants.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have assigned the following  operator to the merchants")]
        [When(@"I assign the following  operator to the merchants")]
        public async Task WhenIAssignTheFollowingOperatorToTheMerchants(DataTable table){
            List<EstateDetails> estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, Guid, AssignOperatorRequest)> requests = table.Rows.ToAssignOperatorRequests(estates);

            List<(EstateDetails, MerchantOperatorResponse)> results = await this.TransactionProcessorSteps.WhenIAssignTheFollowingOperatorToTheMerchants(this.TestingContext.AccessToken, requests);

            foreach ((EstateDetails, MerchantOperatorResponse) result in results){
                this.TestingContext.Logger.LogInformation($"Operator {result.Item2.Name} assigned to Estate {result.Item1.EstateName}");
            }
        }

        /// <summary>
        /// Whens the i create the following estates.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have created the following estates")]
        [When(@"I create the following estates")]
        public async Task WhenICreateTheFollowingEstates(DataTable table){
            List<CreateEstateRequest> requests = table.Rows.ToCreateEstateRequests();
            
            List<EstateResponse> verifiedEstates = await this.TransactionProcessorSteps.WhenICreateTheFollowingEstatesX(this.TestingContext.AccessToken, requests);

            foreach (EstateResponse verifiedEstate in verifiedEstates){
                this.TestingContext.AddEstateDetails(verifiedEstate.EstateId, verifiedEstate.EstateName, verifiedEstate.EstateReference);
                this.TestingContext.Logger.LogInformation($"Estate {verifiedEstate.EstateName} created with Id {verifiedEstate.EstateId}");
            }
        }

        /// <summary>
        /// Whens the i create the following merchants.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given("I create the following merchants")]
        [When(@"I create the following merchants")]
        public async Task WhenICreateTheFollowingMerchants(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails estate, CreateMerchantRequest)> requests = table.Rows.ToCreateMerchantRequests(estates);

            List<MerchantResponse> verifiedMerchants = await this.TransactionProcessorSteps.WhenICreateTheFollowingMerchants(this.TestingContext.AccessToken, requests);

            foreach (MerchantResponse verifiedMerchant in verifiedMerchants){
                EstateDetails1 estateDetails = this.TestingContext.GetEstateDetails(verifiedMerchant.EstateId);
                estateDetails.EstateDetails.AddMerchant(verifiedMerchant);
                this.TestingContext.Logger.LogInformation($"Merchant {verifiedMerchant.MerchantName} created with Id {verifiedMerchant.MerchantId} for Estate {estateDetails.EstateDetails.EstateName}");
            }
        }

        /// <summary>
        /// Whens the i create the following operators.
        /// </summary>
        /// <param name="table">The table.</param>
        [Given(@"I have created the following operators")]
        [When(@"I create the following operators")]
        public async Task WhenICreateTheFollowingOperators(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails estate, CreateOperatorRequest request)> requests = table.Rows.ToCreateOperatorRequests(estates);

            List<(Guid, EstateOperatorResponse)> results = await this.TransactionProcessorSteps.WhenICreateTheFollowingOperators(this.TestingContext.AccessToken, requests);

            foreach ((Guid, EstateOperatorResponse) result in results){
                this.TestingContext.Logger.LogInformation($"Operator {result.Item2.Name} created with Id {result.Item2.OperatorId} for Estate {result.Item1}");
            }
        }

        [Given("I have assigned the following operators to the estates")]
        public async Task GivenIHaveAssignedTheFollowingOperatorsToTheEstates(DataTable dataTable)
        {
            List<(EstateDetails estate, TransactionProcessor.DataTransferObjects.Requests.Estate.AssignOperatorRequest request)> requests = dataTable.Rows.ToAssignOperatorToEstateRequests(this.TestingContext.Estates.Select(e => e.EstateDetails).ToList());

            await this.TransactionProcessorSteps.GivenIHaveAssignedTheFollowingOperatorsToTheEstates(this.TestingContext.AccessToken, requests);

            // TODO Verify
        }

        /// <summary>
        /// Whens the i create the following products.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I create the following Products")]
        public async Task WhenICreateTheFollowingProducts(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, TransactionProcessor.IntegrationTesting.Helpers.Contract, AddProductToContractRequest)> requests = table.Rows.ToAddProductToContractRequest(estates);
            await this.TransactionProcessorSteps.WhenICreateTheFollowingProducts(this.TestingContext.AccessToken, requests);
        }

        /// <summary>
        /// Whens the i create the following security users.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I create the following security users")]
        [Given("I have created the following security users")]
        public async Task WhenICreateTheFollowingSecurityUsers(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<CreateNewUserRequest> createUserRequests = table.Rows.ToCreateNewUserRequests(estates);
            await this.TransactionProcessorSteps.WhenICreateTheFollowingSecurityUsers(this.TestingContext.AccessToken, createUserRequests, estates);
        }

        [Given(@"I have created the following security users for voucher redemption")]
        public async Task GivenIHaveCreatedTheFollowingSecurityUsersForVoucherRedemption(DataTable table){
            List<CreateUserRequest> createUserRequests = table.Rows.ToAclCreateUserRequests(this.TestingContext.Estates);
            await this.SecurityServiceSteps.GivenICreateTheFollowingUsers(createUserRequests, CancellationToken.None);
        }
        
        /// <summary>
        /// Whens the i perform the following reconciliations.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I perform the following reconciliations")]
        public async Task WhenIPerformTheFollowingReconciliations(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, Guid, String, SerialisedMessage)> serialisedMessages = table.Rows.ToSerialisedMessages(estates);
            List<(EstateDetails, String, Guid, String, TransactionRequestMessage)> requestMessages = ReqnrollExtensions.ToACLSerialisedMessages(SharedSteps.ApplicationVersion,
                                                                                                                                                serialisedMessages,
                                                                                                                                                this.TestingContext.Estates);

            foreach ((EstateDetails, String, Guid, String, TransactionRequestMessage) transactionRequestMessage in requestMessages){
                await this.AclSteps.SendAclRequestMessage(transactionRequestMessage, CancellationToken.None);
            }
        }

        /// <summary>
        /// Whens the i perform the following transactions.
        /// </summary>
        /// <param name="table">The table.</param>
        [When(@"I perform the following transactions")]
        public async Task WhenIPerformTheFollowingTransactions(DataTable table){
            var estates = this.TestingContext.Estates.Select(e => e.EstateDetails).ToList();
            List<(EstateDetails, Guid, String, SerialisedMessage)> serialisedMessages = table.Rows.ToSerialisedMessages(estates);
            List<(EstateDetails, String, Guid, String, TransactionRequestMessage)> requestMessages = ReqnrollExtensions.ToACLSerialisedMessages(SharedSteps.ApplicationVersion,
                                                                                                                                                serialisedMessages,
                                                                                                                                                this.TestingContext.Estates);

            foreach ((EstateDetails, String, Guid, String, TransactionRequestMessage) transactionRequestMessage in requestMessages){
                await this.AclSteps.SendAclRequestMessage(transactionRequestMessage, CancellationToken.None);
            }
        }

        [When(@"I redeem the voucher for Estate '([^']*)' and Merchant '([^']*)' transaction number (.*) the voucher balance will be (.*)")]
        public async Task WhenIRedeemTheVoucherForEstateAndMerchantTransactionNumberTheVoucherBalanceWillBe(String estateName, String merchantName, Int32 transactionNumber, Int32 balance){
            await this.AclSteps.WhenIRedeemTheVoucherForEstateAndMerchantTransactionNumberTheVoucherBalanceWillBe(this.TestingContext.AccessToken, estateName, merchantName, transactionNumber, balance, this.TestingContext.Estates);
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