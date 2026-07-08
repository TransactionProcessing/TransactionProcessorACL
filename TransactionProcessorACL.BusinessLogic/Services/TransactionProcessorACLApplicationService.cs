using System.Linq;
using SecurityService.DataTransferObjects;
using Shared.Exceptions;
using Shared.Results;
using SimpleResults;
using TransactionProcessor.DataTransferObjects.Requests.Merchant;
using TransactionProcessor.DataTransferObjects.Responses.Contract;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using TransactionProcessorACL.BusinessLogic.BackendAPI;
    using TransactionProcessorACL.DataTransferObjects.Requests;
    using Models;
    using SecurityService.Client;
    using Shared.General;
    using Shared.Logger;
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    public class TransactionProcessorACLApplicationService : ITransactionProcessorACLApplicationService
    {
        private readonly ISecurityServiceClient SecurityServiceClient;

        private readonly ITransactionProcessorClient TransactionProcessorClient;

        private readonly IEstateReportingApiClient EstateReportingApiClient;

        public TransactionProcessorACLApplicationService(ITransactionProcessorClient transactionProcessorClient,
                                                         ISecurityServiceClient securityServiceClient,
                                                         IEstateReportingApiClient estateReportingApiClient)
        {
            this.TransactionProcessorClient = transactionProcessorClient;
            this.SecurityServiceClient = securityServiceClient;
            this.EstateReportingApiClient = estateReportingApiClient;
        }
        
        public async Task<Result<ProcessLogonTransactionResponse>> ProcessLogonTransaction(Guid estateId,
                                                                                           Guid merchantId,
                                                                                           DateTime transactionDateTime,
                                                                                           String transactionNumber,
                                                                                           String deviceIdentifier,
                                                                                           CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            LogonTransactionRequest logonTransactionRequest = CreateLogonRequestMessage(estateId, merchantId, transactionDateTime, transactionNumber, deviceIdentifier);

            try {
                Result<LogonTransactionResponse> transactionResult = await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, logonTransactionRequest, cancellationToken);

                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                
                return Result.Success(CreateProcessLogonTransactionResponse(estateId, merchantId, transactionResult.Data));
            }
            catch (Exception ex) {
                return Result.Success(CreateLogonErrorResponse(estateId, merchantId, ex));
            }
        }

        public async Task<Result<ProcessSaleTransactionResponse>> ProcessSaleTransaction((Guid estateId, Guid merchantId) merchantData,
                                                                                         DateTime transactionDateTime,
                                                                                         String transactionNumber,
                                                                                         String deviceIdentifier,
                                                                                         String customerEmailAddress,
                                                                                         (Guid operatorId, Guid contractId, Guid productId) productData,
                                                                                         Dictionary<String, String> additionalRequestMetadata, CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            TokenResponse accessToken = accessTokenResult.Data;

            SaleTransactionRequest saleTransactionRequest = this.BuildSaleTransactionRequest(merchantData,
                                                                                            transactionNumber,
                                                                                            deviceIdentifier,
                                                                                            transactionDateTime,
                                                                                            productData,
                                                                                            customerEmailAddress,
                                                                                            additionalRequestMetadata);

            ProcessSaleTransactionResponse response = null;

            try
            {
                Result<SaleTransactionResponse> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, saleTransactionRequest, cancellationToken);
                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);

                response = this.CreateSaleTransactionResponse(transactionResult.Data, merchantData.estateId, merchantData.merchantId);
            }
            catch (Exception ex)
            {
                response = this.CreateSaleTransactionErrorResponse(merchantData.estateId, merchantData.merchantId, ex);
            }

            return Result.Success(response);
        }

        private async Task<Result<TokenResponse>> GetAccessToken(CancellationToken cancellationToken)
        {
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            return await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
        }

        private SaleTransactionRequest BuildSaleTransactionRequest((Guid estateId, Guid merchantId) merchantData,
                                                                   String transactionNumber,
                                                                   String deviceIdentifier, 
                                                                   DateTime transactionDateTime,
                                                                   (Guid operatorId, Guid contractId, Guid productId) productData,
                                                                   String customerEmailAddress,
                                                                   Dictionary<String, String> additionalRequestMetadata)
        {
            return new SaleTransactionRequest
            {
                TransactionNumber = transactionNumber,
                DeviceIdentifier = deviceIdentifier,
                TransactionDateTime = transactionDateTime,
                TransactionType = "SALE",
                OperatorId = productData.operatorId,
                CustomerEmailAddress = customerEmailAddress,
                TransactionSource = 1,
                ContractId = productData.contractId,
                ProductId = productData.productId,
                AdditionalTransactionMetadata = additionalRequestMetadata,
                EstateId = merchantData.estateId,
                MerchantId = merchantData.merchantId,
            };
        }


        private ProcessSaleTransactionResponse CreateSaleTransactionResponse(SaleTransactionResponse saleTransactionResponse,
                                                                             Guid estateId,
                                                                             Guid merchantId)
        {
            return new ProcessSaleTransactionResponse
            {
                ResponseCode = saleTransactionResponse.ResponseCode,
                ResponseMessage = saleTransactionResponse.ResponseMessage,
                EstateId = estateId,
                MerchantId = merchantId,
                AdditionalTransactionMetadata = saleTransactionResponse.AdditionalTransactionMetadata,
                TransactionId = saleTransactionResponse.TransactionId,
            };
        }

        private ProcessSaleTransactionResponse CreateSaleTransactionErrorResponse(Guid estateId,
                                                                                  Guid merchantId,
                                                                                  Exception ex)
        {
            return new ProcessSaleTransactionResponse
            {
                ResponseCode = "0001", // Request Message error
                ResponseMessage = "Process Sale Failed",
                EstateId = estateId,
                MerchantId = merchantId,
                ErrorMessages = ex.GetExceptionMessages()
            };
        }

        public async Task<Result<ProcessReconciliationResponse>> ProcessReconciliation(Guid estateId,
                                                                                       Guid merchantId,
                                                                                       DateTime transactionDateTime,
                                                                                       String deviceIdentifier,
                                                                                       Int32 transactionCount,
                                                                                       Decimal transactionValue,
                                                                                       CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            ReconciliationRequest reconciliationRequest = CreateReconciliationRequestMessage(estateId,
                                                                                            merchantId,
                                                                                            transactionDateTime,
                                                                                            deviceIdentifier,
                                                                                            transactionCount,
                                                                                            transactionValue);

            try
            {
                Result<ReconciliationResponse> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, reconciliationRequest, cancellationToken);
                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                return Result.Success(CreateProcessReconciliationResponse(transactionResult.Data));
            }
            catch (Exception ex)
            {
                return Result.Success(CreateReconciliationErrorResponse(estateId, merchantId, ex));
            }
        }

        public async Task<Result<GetVoucherResponse>> GetVoucher(Guid estateId,
                                                                 Guid contractId,
                                                                 String voucherCode,
                                                                 CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;

            try {
                Result<TransactionProcessor.DataTransferObjects.GetVoucherResponse> getVoucherResult = await this.TransactionProcessorClient.GetVoucherByCode(accessToken.AccessToken, estateId, voucherCode, cancellationToken);
                if (getVoucherResult.IsFailed) {
                    return ResultHelpers.CreateFailure(getVoucherResult);
                }

                return Result.Success(CreateGetVoucherSuccessResponse(estateId, contractId, getVoucherResult.Data));
            }
            catch (Exception ex) {
                return Result.Failure(ex.GetExceptionMessages());
            }
        }

        public async Task<Result<RedeemVoucherResponse>> RedeemVoucher(Guid estateId,
                                                                       Guid contractId,
                                                                       String voucherCode,
                                                                       CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            
            TokenResponse accessToken = accessTokenResult.Data;

            try {
                RedeemVoucherRequest redeemVoucherRequest = new() { EstateId = estateId, VoucherCode = voucherCode };

                Result redeemVoucherResponseResult = await this.TransactionProcessorClient.RedeemVoucher(accessToken.AccessToken, redeemVoucherRequest, cancellationToken);

                if (redeemVoucherResponseResult.IsFailed) {
                    return ResultHelpers.CreateFailure(redeemVoucherResponseResult);
                }

                return Result.Success(new RedeemVoucherResponse {
                    ResponseCode = "0000", // Success
                    ResponseMessage = "SUCCESS",
                    ContractId = contractId,
                    EstateId = estateId
                });
            }
            catch (Exception ex) {
                return Result.Failure(ex.GetExceptionMessages());
            }
        }

        public async Task<Result<List<Models.ContractResponse>>> GetMerchantContracts(Guid estateId,
                                                                                      Guid merchantId,
                                                                                      CancellationToken cancellationToken) {
            Logger.LogInformation($"GetMerchantContracts: {estateId} {merchantId}");

            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;

            Result<List<TransactionProcessor.DataTransferObjects.Responses.Contract.ContractResponse>> result = await this.TransactionProcessorClient.GetMerchantContracts(accessToken.AccessToken, estateId, merchantId, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant contracts {result.Message}");

            Logger.LogInformation($"Got the merchant contracts {result.Data.Count}");
            List<Models.ContractResponse> models = new();

            foreach (TransactionProcessor.DataTransferObjects.Responses.Contract.ContractResponse contractResponse in result.Data) {
                Logger.LogInformation($"Processing contract {contractResponse.OperatorName}");

                ContractResponse contractModel = new ContractResponse {
                    ContractId = contractResponse.ContractId,
                    ContractReportingId = contractResponse.ContractReportingId,
                    Description = contractResponse.Description,
                    EstateId = contractResponse.EstateId,
                    EstateReportingId = contractResponse.EstateReportingId,
                    OperatorId = contractResponse.OperatorId,
                    OperatorName = contractResponse.OperatorName,
                    Products = new()
                };

                foreach (TransactionProcessor.DataTransferObjects.Responses.Contract.ContractProduct contractResponseProduct in contractResponse.Products) {
                    Logger.LogInformation($"Processing contract product {contractResponseProduct.DisplayText}");
                    ContractProduct productModel = new ContractProduct {
                        Value = contractResponseProduct.Value,
                        DisplayText = contractResponseProduct.DisplayText,
                        Name = contractResponseProduct.Name,
                        ProductId = contractResponseProduct.ProductId,
                        ProductReportingId = contractResponseProduct.ProductReportingId,
                        ProductType = contractResponseProduct.ProductType switch {
                            TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.BillPayment => ProductType.BillPayment,
                            TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.MobileTopup => ProductType.MobileTopup,
                            TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.Voucher => ProductType.Voucher,
                            _ => ProductType.NotSet
                        },
                        TransactionFees = new()
                    };
                    
                    contractModel.Products.Add(productModel);
                }

                models.Add(contractModel);
            }

            return Result.Success(models);
        }

        public async Task<Result<MerchantResponse>> GetMerchant(Guid estateId,
                                                                Guid merchantId,
                                                                CancellationToken cancellationToken) {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            
            Result<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantResponse> result = await this.TransactionProcessorClient.GetMerchant(accessToken.AccessToken, estateId, merchantId, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant {result.Message}");
            
            MerchantResponse merchantResponse = ResponseFactory.Build(result.Data);

            return Result.Success(merchantResponse);
        }

        public async Task<Result<MerchantScheduleResponse>> GetMerchantSchedule(Guid estateId,
                                                                                Guid merchantId,
                                                                                Int32 year,
                                                                                CancellationToken cancellationToken) {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed)
            {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;

            Result<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantScheduleResponse> result = await this.TransactionProcessorClient.GetMerchantSchedule(accessToken.AccessToken, estateId, merchantId, year, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant schedule {result.Message}");

            MerchantScheduleResponse merchantScheduleResponse = ResponseFactory.Build(result.Data);

            return Result.Success(merchantScheduleResponse);
        }

        public async Task<Result<MerchantDailyPerformanceSummaryResponse>> GetMerchantDailyPerformanceSummary(Guid estateId,
                                                                                                               MerchantDailyPerformanceSummaryRequest request,
                                                                                                               CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            BackendAPI.DataTransferObjects.MerchantDailyPerformanceSummaryRequest apiRequest = new BackendAPI.DataTransferObjects.MerchantDailyPerformanceSummaryRequest { StartDate = request.StartDate, EndDate = request.EndDate, MerchantReportingId = request.MerchantReportingId };

            TokenResponse accessToken = accessTokenResult.Data;
            Result<BackendAPI.DataTransferObjects.MerchantDailyPerformanceSummaryResponse> apiResponse = await this.EstateReportingApiClient.GetMerchantDailyPerformanceSummary(accessToken.AccessToken, estateId, apiRequest, cancellationToken);

            if (apiResponse.IsFailed)
                return ResultHelpers.CreateFailure(apiResponse);

            MerchantDailyPerformanceSummaryResponse response = new MerchantDailyPerformanceSummaryResponse {
                Metrics = apiResponse.Data.Metrics.Select(m => new MetricItem { Category = m.Category, Description = m.Description, Title = m.Title, Value = m.Value }).ToList(),
                DrillDownTransactions = apiResponse.Data.DrillDownTransactions.Select(d => new DrillDownTransaction {
                    Amount = d.Amount,
                    Product = d.Product,
                    Reference = d.Reference,
                    Status = d.Status,
                    TransactionDateTime = d.TransactionDateTime
                }).ToList()
            };

            return response;
        }

        public async Task<Result<MerchantTransactionMixSummaryResponse>> GetMerchantTransactionMixSummary(Guid estateId,
                                                                                                          MerchantTransactionMixSummaryRequest request,
                                                                                                          CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            var apiRequest = new BackendAPI.DataTransferObjects.TransactionMixSummaryRequest {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Breakdown = Enum.Parse<BackendAPI.DataTransferObjects.TransactionMixBreakdown>(request.Breakdown.ToString()),
                Measure = Enum.Parse<BackendAPI.DataTransferObjects.TransactionMixMeasure>(request.Measure.ToString()),
                MerchantReportingId = request.MerchantReportingId,
                TopN = request.TopN
            };

            TokenResponse accessToken = accessTokenResult.Data;
            var apiResponse = await this.EstateReportingApiClient.GetMerchantTransactionMixSummary(accessToken.AccessToken, estateId, apiRequest, cancellationToken);

            if (apiResponse.IsFailed)
                return ResultHelpers.CreateFailure(apiResponse);

            MerchantTransactionMixSummaryResponse response = new MerchantTransactionMixSummaryResponse() {
                Breakdown = Enum.Parse<Models.TransactionMixBreakdown>(apiResponse.Data.Breakdown.ToString()),
                Measure = Enum.Parse<Models.TransactionMixMeasure>(apiResponse.Data.Measure.ToString()),
                FromDate = apiResponse.Data.FromDate,
                ToDate = apiResponse.Data.ToDate,
                TotalCount = apiResponse.Data.TotalCount,
                TotalValue = apiResponse.Data.TotalValue,
                Items = apiResponse.Data.Groups.Select(g => new TransactionMixSummaryItem { Value = g.TransactionValue, Count = g.TransactionCount, Key = g.GroupKey, Label = g.GroupName }).ToList(),
                DrillDownTransactions = apiResponse.Data.Transactions.Select(t => new TransactionMixDrillDownTransaction {
                    Amount = t.Value,
                    Operator = t.Operator,
                    Product = t.Product,
                    Reference = t.SettlementReference,
                    Status = t.Status,
                    TransactionDateTime = t.DateTime,
                    TransactionType = t.Type
                }).ToList()

            };

            return response;
        }

        public async Task<Result<RecentActivityReceiptSearchResponse>> GetRecentActivityReceiptSearch(Guid estateId,
                                                                                                       RecentActivityReceiptSearchRequest request,
                                                                                                       CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            var apiRequest = new BackendAPI.DataTransferObjects.RecentActivityReceiptSearchRequest
            {
                MerchantReportingId = request.MerchantReportingId,
                ReportDate = request.ReportDate.Date,
                SearchText = string.IsNullOrWhiteSpace(request.SearchText) ? null : request.SearchText.Trim(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            TokenResponse accessToken = accessTokenResult.Data;
            var apiResponse = await this.EstateReportingApiClient.GetRecentActivityReceiptSearch(accessToken.AccessToken, estateId, apiRequest, cancellationToken);

            if (apiResponse.IsFailed)
                return ResultHelpers.CreateFailure(apiResponse);

            RecentActivityReceiptSearchResponse response = new()
            {
                ReportDate = apiResponse.Data.ReportDate,
                PageNumber = apiResponse.Data.PageNumber,
                PageSize = apiResponse.Data.PageSize,
                TotalCount = apiResponse.Data.TotalCount,
                Items = apiResponse.Data.Items.Select(i => new RecentActivityReceiptSearchItem
                {
                    Amount = i.Amount,
                    Operator = i.Operator,
                    Product = i.Product,
                    Reference = i.Reference,
                    ReceiptReference = i.ReceiptReference,
                    Status = i.Status,
                    TransactionDateTime = i.TransactionDateTime,
                    TransactionType = i.TransactionType
                }).ToList()
            };

            return response;
        }

        public async Task<Result<ResendReceiptResponse>> ResendReceipt(Guid estateId,
                                                                       Guid merchantId,
                                                                       String reference,
                                                                       String recipientEmailAddress,
                                                                       CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            if (string.IsNullOrWhiteSpace(reference)) {
                return Result.Invalid("A transaction reference or receipt reference is required.");
            }

            if (string.IsNullOrWhiteSpace(recipientEmailAddress)) {
                return Result.Invalid("Recipient email address is required.");
            }

            try {
                _ = new System.Net.Mail.MailAddress(recipientEmailAddress);
            }
            catch (FormatException) {
                return Result.Invalid("Recipient email address is not valid.");
            }

            Result<ResendReceiptResponse> resendResult = await this.InvokeResendReceipt(accessTokenResult.Data.AccessToken,
                                                                                         estateId,
                                                                                         merchantId,
                                                                                         reference,
                                                                                         recipientEmailAddress,
                                                                                         cancellationToken);

            if (resendResult.IsFailed) {
                return resendResult;
            }

            return resendResult;
        }

        private async Task<Result<ResendReceiptResponse>> InvokeResendReceipt(String accessToken,
                                                                              Guid estateId,
                                                                              Guid merchantId,
                                                                              String reference,
                                                                              String recipientEmailAddress,
                                                                              CancellationToken cancellationToken)
        {
            System.Reflection.MethodInfo? method = FindResendEmailReceiptMethod();
            if (method == null) {
                return Result.Failure("Receipt resend is not supported by the transaction client.");
            }

            object?[]? args = BuildResendReceiptArguments(method, accessToken, estateId, merchantId, reference, recipientEmailAddress, cancellationToken);
            if (args == null) {
                return Result.Failure("Unable to prepare receipt resend request.");
            }

            object? invocationResult = method.Invoke(this.TransactionProcessorClient, args);
            if (invocationResult is not Task task) {
                return Result.Failure("Receipt resend call did not return a task.");
            }

            await task.ConfigureAwait(false);
            return MapResendReceiptInvocationResult(task, reference);
        }

        private static System.Reflection.MethodInfo? FindResendEmailReceiptMethod()
        {
            return typeof(ITransactionProcessorClient)
                .GetMethods()
                .SingleOrDefault(m => string.Equals(m.Name, "ResendEmailReceipt", StringComparison.Ordinal));
        }

        private static object?[]? BuildResendReceiptArguments(System.Reflection.MethodInfo method,
                                                              String accessToken,
                                                              Guid estateId,
                                                              Guid merchantId,
                                                              String reference,
                                                              String recipientEmailAddress,
                                                              CancellationToken cancellationToken)
        {
            System.Reflection.ParameterInfo[] parameters = method.GetParameters();
            object?[] args = new object?[parameters.Length];
            ResendReceiptInvocationState state = new();

            for (int index = 0; index < parameters.Length; index++) {
                object? argument = BuildResendReceiptArgument(parameters[index],
                                                              state,
                                                              accessToken,
                                                              estateId,
                                                              merchantId,
                                                              reference,
                                                              recipientEmailAddress,
                                                              cancellationToken);
                if (argument == ResendReceiptInvocationState.UnableToBuildArgument) {
                    return null;
                }

                args[index] = argument;
            }

            return args;
        }

        private static object? BuildResendReceiptArgument(System.Reflection.ParameterInfo parameter,
                                                          ResendReceiptInvocationState state,
                                                          String accessToken,
                                                          Guid estateId,
                                                          Guid merchantId,
                                                          String reference,
                                                          String recipientEmailAddress,
                                                          CancellationToken cancellationToken)
        {
            if (parameter.ParameterType == typeof(String)) {
                return accessToken;
            }

            if (parameter.ParameterType == typeof(Guid)) {
                return state.GetNextGuidArgument(estateId, merchantId);
            }

            if (parameter.ParameterType == typeof(CancellationToken)) {
                return cancellationToken;
            }

            return state.GetResendReceiptRequest(parameter.ParameterType, estateId, merchantId, reference, recipientEmailAddress);
        }

        private static Result<ResendReceiptResponse> MapResendReceiptInvocationResult(Task task, String reference)
        {
            object? taskResult = task.GetType().GetProperty("Result")?.GetValue(task);
            if (taskResult == null) {
                return Result.Failure("Receipt resend call returned no result.");
            }

            if (IsFailedResult(taskResult, out String? message, out String? status)) {
                return MapResendReceiptFailure(status, message);
            }

            object? data = taskResult.GetType().GetProperty("Data")?.GetValue(taskResult);
            return Result.Success(MapResendReceiptResponse(data, reference));
        }

        private static bool IsFailedResult(object taskResult, out String? message, out String? status)
        {
            message = taskResult.GetType().GetProperty("Message")?.GetValue(taskResult)?.ToString();
            status = taskResult.GetType().GetProperty("Status")?.GetValue(taskResult)?.ToString();
            return (bool)(taskResult.GetType().GetProperty("IsFailed")?.GetValue(taskResult) ?? true);
        }

        private static Result<ResendReceiptResponse> MapResendReceiptFailure(String? status, String? message)
        {
            if (string.Equals(status, "NotFound", StringComparison.OrdinalIgnoreCase)) {
                return Result.NotFound(message ?? "Receipt could not be found.");
            }

            if (string.Equals(status, "Invalid", StringComparison.OrdinalIgnoreCase)) {
                return Result.Invalid(message ?? "Receipt resend request was invalid.");
            }

            return Result.Failure(message ?? "Receipt resend failed.");
        }

        private static void PopulateResendReceiptRequest(object request,
                                                         Guid estateId,
                                                         Guid merchantId,
                                                         String reference,
                                                         String recipientEmailAddress)
        {
            Type requestType = request.GetType();
            SetIfPresent(requestType, request, "EstateId", estateId);
            SetIfPresent(requestType, request, "MerchantId", merchantId);
            SetIfPresent(requestType, request, "Reference", reference);
            SetIfPresent(requestType, request, "ReceiptReference", reference);
            SetIfPresent(requestType, request, "TransactionReference", reference);
            SetIfPresent(requestType, request, "RecipientEmail", recipientEmailAddress);
            SetIfPresent(requestType, request, "RecipientEmailAddress", recipientEmailAddress);
            SetIfPresent(requestType, request, "EmailAddress", recipientEmailAddress);
        }

        private static void SetIfPresent(Type requestType, object request, String propertyName, object value)
        {
            System.Reflection.PropertyInfo? property = requestType.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
            if (property == null || property.CanWrite == false) {
                return;
            }

            property.SetValue(request, value);
        }

        private static ResendReceiptResponse MapResendReceiptResponse(object? data, String reference)
        {
            ResendReceiptResponse response = new()
            {
                Success = true,
                Message = "Receipt resend requested.",
                Reference = reference
            };

            if (data == null) {
                return response;
            }

            Type responseType = data.GetType();
            response.Message = GetStringProperty(responseType, data, "Message") ?? response.Message;
            response.Reference = GetStringProperty(responseType, data, "Reference") ?? response.Reference;
            response.ReceiptReference = GetStringProperty(responseType, data, "ReceiptReference");
            response.TransactionReference = GetStringProperty(responseType, data, "TransactionReference");

            if (string.IsNullOrWhiteSpace(response.Reference) && string.IsNullOrWhiteSpace(response.ReceiptReference) == false) {
                response.Reference = response.ReceiptReference;
            }

            if (string.IsNullOrWhiteSpace(response.Reference) && string.IsNullOrWhiteSpace(response.TransactionReference) == false) {
                response.Reference = response.TransactionReference;
            }

            return response;
        }

        private static string? GetStringProperty(Type type, object instance, String propertyName)
        {
            System.Reflection.PropertyInfo? property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
            return property?.GetValue(instance)?.ToString();
        }

        private sealed class ResendReceiptInvocationState
        {
            public static readonly object UnableToBuildArgument = new();

            private bool estateAssigned;
            private bool merchantAssigned;
            private object? request;

            public object? GetNextGuidArgument(Guid estateId, Guid merchantId)
            {
                if (this.estateAssigned == false) {
                    this.estateAssigned = true;
                    return estateId;
                }

                if (this.merchantAssigned == false) {
                    this.merchantAssigned = true;
                    return merchantId;
                }

                return Guid.Empty;
            }

            public object? GetResendReceiptRequest(Type requestType,
                                                   Guid estateId,
                                                   Guid merchantId,
                                                   String reference,
                                                   String recipientEmailAddress)
            {
                this.request ??= System.Activator.CreateInstance(requestType);
                if (this.request == null) {
                    return UnableToBuildArgument;
                }

                PopulateResendReceiptRequest(this.request, estateId, merchantId, reference, recipientEmailAddress);
                return this.request;
            }
        }

        private static ProcessReconciliationResponse CreateProcessReconciliationResponse(ReconciliationResponse reconciliationResponse)
        {
            return new ProcessReconciliationResponse
            {
                ResponseCode = reconciliationResponse.ResponseCode,
                ResponseMessage = reconciliationResponse.ResponseMessage,
                TransactionId = reconciliationResponse.TransactionId,
            };
        }

        private static GetVoucherResponse CreateGetVoucherSuccessResponse(Guid estateId,
                                                                          Guid contractId,
                                                                          TransactionProcessor.DataTransferObjects.GetVoucherResponse getVoucherResponse)
        {
            return new GetVoucherResponse
            {
                ResponseCode = "0000", // Success
                ContractId = contractId,
                EstateId = estateId,
                ExpiryDate = getVoucherResponse.ExpiryDate,
                Value = getVoucherResponse.Value,
                VoucherCode = getVoucherResponse.VoucherCode,
                VoucherId = getVoucherResponse.VoucherId,
                Balance = getVoucherResponse.Balance
            };
        }
        
        private static ProcessReconciliationResponse CreateReconciliationErrorResponse(Guid estateId,
                                                                                       Guid merchantId,
                                                                                       Exception ex)
        {
            return new ProcessReconciliationResponse
            {
                ResponseCode = "0001", // Request Message error
                ResponseMessage = "Process Reconciliation Failed",
                EstateId = estateId,
                MerchantId = merchantId,
                ErrorMessages = ex.GetExceptionMessages()
            };
        }

        private static ReconciliationRequest CreateReconciliationRequestMessage(Guid estateId,
                                                                                Guid merchantId,
                                                                                DateTime transactionDateTime,
                                                                                String deviceIdentifier,
                                                                                Int32 transactionCount,
                                                                                Decimal transactionValue)
        {
            ReconciliationRequest reconciliationRequest = new ReconciliationRequest
            {
                DeviceIdentifier = deviceIdentifier,
                TransactionDateTime = transactionDateTime,
                TransactionCount = transactionCount,
                TransactionValue = transactionValue,
                EstateId = estateId,
                MerchantId = merchantId
            };

            return reconciliationRequest;
        }

        private static ProcessLogonTransactionResponse CreateProcessLogonTransactionResponse(Guid estateId,
                                                                                             Guid merchantId,
                                                                                             LogonTransactionResponse logonTransactionResponse)
        {
            return new ProcessLogonTransactionResponse
            {
                ResponseCode = logonTransactionResponse.ResponseCode,
                ResponseMessage = logonTransactionResponse.ResponseMessage,
                EstateId = estateId,
                MerchantId = merchantId,
                TransactionId = logonTransactionResponse.TransactionId,
            };
        }

        private static ProcessLogonTransactionResponse CreateLogonErrorResponse(Guid estateId,
                                                                                Guid merchantId,
                                                                                Exception ex)
        {
            return new ProcessLogonTransactionResponse
            {
                ResponseCode = "0001", // Request Message error
                ResponseMessage = "Process Logon Failed",
                EstateId = estateId,
                MerchantId = merchantId,
                ErrorMessages = ex.GetExceptionMessages()
            };
        }

        private static LogonTransactionRequest CreateLogonRequestMessage(Guid estateId,
                                                                         Guid merchantId,
                                                                         DateTime transactionDateTime,
                                                                         String transactionNumber,
                                                                         String deviceIdentifier)
        {
            LogonTransactionRequest logonTransactionRequest = new LogonTransactionRequest
            {
                TransactionNumber = transactionNumber,
                DeviceIdentifier = deviceIdentifier,
                TransactionDateTime = transactionDateTime,
                TransactionType = "LOGON",
                EstateId = estateId,
                MerchantId = merchantId
            };
            
            return logonTransactionRequest;
        }
    }
}
