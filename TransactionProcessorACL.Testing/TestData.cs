﻿namespace TransactionProcessorACL.Testing
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Newtonsoft.Json;
    using SecurityService.DataTransferObjects.Responses;
    using TransactionProcessor.DataTransferObjects;
    using TransactionProcessorACL.BusinessLogic.Requests;
    using TransactionProcessorACL.DataTransferObjects;
    using GetVoucherResponse = TransactionProcessor.DataTransferObjects.GetVoucherResponse;
    using RedeemVoucherResponse = TransactionProcessor.DataTransferObjects.RedeemVoucherResponse;

    /// <summary>
    /// 
    /// </summary>
    public class TestData
    {
        #region Fields

        /// <summary>
        /// The estate identifier
        /// </summary>
        public static Guid EstateId = Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3");

        /// <summary>
        /// The device identifier
        /// </summary>
        public static String DeviceIdentifier = "12345678";
        
        /// <summary>
        /// The transaction date time
        /// </summary>
        public static DateTime TransactionDateTime = DateTime.Now;

        /// <summary>
        /// The transaction number
        /// </summary>
        public static String TransactionNumber = "0001";

        /// <summary>
        /// The logon transaction request message
        /// </summary>
        public static LogonTransactionRequestMessage LogonTransactionRequestMessage = new LogonTransactionRequestMessage
                                                                                      {
                                                                                          DeviceIdentifier = TestData.DeviceIdentifier,
                                                                                          TransactionDateTime = TestData.TransactionDateTime,
                                                                                          TransactionNumber = TestData.TransactionNumber
                                                                                      };

        /// <summary>
        /// The merchant identifier
        /// </summary>
        public static Guid MerchantId = Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3");

        /// <summary>
        /// The process logon transaction request
        /// </summary>
        public static TransactionCommands.ProcessLogonTransactionCommand ProcessLogonTransactionCommand =
            new(TestData.EstateId,
                                                  TestData.MerchantId,
                                                  TestData.TransactionDateTime,
                                                  TestData.TransactionNumber,
                                                  TestData.DeviceIdentifier);

        public static String ResponseCode = "0000";
        public static String ExceptionErrorResponseCode = "0001";

        public static String ResponseMessage = "SUCCESS";
        public static String LogonExceptionResponseMessage = "Process Logon Failed";
        public static String SaleExceptionResponseMessage = "Process Sale Failed";
        public static String ReconciliationExceptionResponseMessage = "Process Reconciliation Failed";
        public static String VoucherExceptionResponseMessage = "Get Voucher Failed";
        public static String RedeemVoucherExceptionResponseMessage = "Redeem Voucher Failed";

        public static ProcessLogonTransactionResponse ProcessLogonTransactionResponse = new ProcessLogonTransactionResponse
                                                                                        {
                                                                                            ResponseMessage = TestData.ResponseMessage,
                                                                                            ResponseCode = TestData.ResponseCode,
                                                                                            EstateId = TestData.EstateId,
                                                                                            MerchantId = TestData.MerchantId
                                                                                        };

        public static LogonTransactionResponse ClientLogonTransactionResponse = new LogonTransactionResponse
                                                                                        {
                                                                                            ResponseMessage = TestData.ResponseMessage,
                                                                                            ResponseCode = TestData.ResponseCode,
                                                                                            EstateId = TestData.EstateId,
                                                                                            MerchantId = TestData.MerchantId
                                                                                        };


        public static SerialisedMessage SerialisedMessageResponseLogon = new SerialisedMessage
                                                                    {
                                                                        Metadata = new Dictionary<String, String>
                                                                                   {
                                                                                       {"EstateId", TestData.EstateId.ToString()},
                                                                                       {"MerchantId", TestData.MerchantId.ToString()},
                                                                                   },
                                                                        SerialisedData = JsonConvert.SerializeObject(TestData.ClientLogonTransactionResponse, new JsonSerializerSettings
                                                                                                                                                               {
                                                                                                                                                                   TypeNameHandling = TypeNameHandling.All
                                                                                                                                                               })
                                                                    };

        #endregion

        public static String Token = "{\"access_token\": \"eyJhbGciOiJSUzI1NiIsImtpZCI6IjA4NGZlNTIwZmIzZmVhM2M0MmNmMjBiZWM2OGY1NDg2IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE1Nzc1NTIyMTQsImV4cCI6MTU3NzU1NTgxNCwiaXNzIjoiaHR0cDovLzE5Mi4xNjguMS4xMzM6NTAwMSIsImF1ZCI6WyJlc3RhdGVNYW5hZ2VtZW50IiwidHJhbnNhY3Rpb25Qcm9jZXNzb3IiLCJ0cmFuc2FjdGlvblByb2Nlc3NvckFDTCJdLCJjbGllbnRfaWQiOiJzZXJ2aWNlQ2xpZW50Iiwic2NvcGUiOlsiZXN0YXRlTWFuYWdlbWVudCIsInRyYW5zYWN0aW9uUHJvY2Vzc29yIiwidHJhbnNhY3Rpb25Qcm9jZXNzb3JBQ0wiXX0.JxK6kEvmvuMnL7ktgvv6N52fjqnFG-NSjPcQORIcFb4ravZAk5oNgsnEtjPcOHTXiktcr8i361GlYO1yiSGdfLKtCTaH3lihcbOb1wfQh3bYM_xmlqJUdirerR8ql9lxqBqm2_Q__PDFuFhMd9lAz-iFr3svuOXeQJB5HQ2rtA3xBDDked5SaEs1dMfbBJA6svRq831WCQSJgap2Db7XN7ais7AQhPYUcFOTGs9Qk33rF_k-2dnAzkEITjvgPwim-8YsEQfsbRYgZmIXfjOXcD81Y0G2_grugvc0SOj_hKXd4d62T-zU-mC4opuYauWKYFqV4UB4sf4V4rtLWeDWrA\",\"expires_in\": 3600,\"token_type\": \"Bearer\",\"scope\": \"estateManagement transactionProcessor transactionProcessorACL\"}";

        public static TokenResponse TokenResponse = TokenResponse.Create(TestData.Token);

        public static ProcessSaleTransactionResponse ProcessSaleTransactionResponse = new ProcessSaleTransactionResponse
                                                                                      {
                                                                                          ResponseMessage = TestData.ResponseMessage,
                                                                                          ResponseCode = TestData.ResponseCode,
                                                                                          EstateId = TestData.EstateId,
                                                                                          MerchantId = TestData.MerchantId
        };

        public static SaleTransactionResponse ClientSaleTransactionResponse = new SaleTransactionResponse
                                                                                      {
                                                                                          ResponseMessage = TestData.ResponseMessage,
                                                                                          ResponseCode = TestData.ResponseCode,
                                                                                          EstateId = TestData.EstateId,
                                                                                          MerchantId = TestData.MerchantId
                                                                                      };

        public static SerialisedMessage SerialisedMessageResponseSale = new SerialisedMessage
                                                                        {
                                                                            Metadata = new Dictionary<String, String>
                                                                                       {
                                                                                           {"EstateId", TestData.EstateId.ToString()},
                                                                                           {"MerchantId", TestData.MerchantId.ToString()},
                                                                                       },
                                                                            SerialisedData = JsonConvert.SerializeObject(TestData.ClientSaleTransactionResponse, new JsonSerializerSettings
                                                                                {
                                                                                    TypeNameHandling = TypeNameHandling.All
                                                                                })
                                                                        };

        public static Guid OperatorId = Guid.Parse("02238578-30F1-409C-BC73-1CC0220A0A0D");

        public static Decimal SaleAmount = 1000.00m;

        public static String CustomerAccountNumber = "123456789";

        public static String CustomerEmailAddress = "testcustomer@customer.co.uk";

        public static Guid ContractId = Guid.Parse("362CCDFD-C227-4D6E-884C-D6323E278175");

        public static Guid ProductId = Guid.Parse("B74D71CA-CC5E-4F49-8412-79D36035D68E");

        public static Dictionary<String, String> AdditionalRequestMetadata = new Dictionary<String, String>
                                                                             {
                                                                                 {"Amount", TestData.SaleAmount.ToString()},
                                                                                 {"CustomerAccountNumber", TestData.CustomerAccountNumber}
                                                                             };

        public static TransactionCommands.ProcessSaleTransactionCommand ProcessSaleTransactionCommand = new(TestData.EstateId,
            TestData.MerchantId,
            TestData.TransactionDateTime,
            TestData.TransactionNumber,
            TestData.DeviceIdentifier,
            TestData.OperatorId,
            TestData.CustomerEmailAddress,
            TestData.ContractId,
            TestData.ProductId,
            TestData.AdditionalRequestMetadata);

        public static Int32 ReconciliationTransactionCount = 1;

        public static Decimal ReconciliationTransactionValue = 100.00m;

        public static TransactionCommands.ProcessReconciliationCommand ProcessReconciliationCommand = new(TestData.EstateId,
             TestData.MerchantId,
             TestData.TransactionDateTime,
             TestData.DeviceIdentifier,
             TestData.ReconciliationTransactionCount,
             TestData.ReconciliationTransactionValue);

        public static ProcessReconciliationResponse ProcessReconciliationResponse = new ProcessReconciliationResponse
        {
                                                                                        ResponseMessage = TestData.ResponseMessage,
                                                                                        ResponseCode = TestData.ResponseCode,
                                                                                        EstateId = TestData.EstateId,
                                                                                        MerchantId = TestData.MerchantId
        };

        public static TransactionProcessor.DataTransferObjects.ReconciliationResponse ClientReconciliationResponse = new ReconciliationResponse
                                                                                    {
                                                                                        ResponseMessage = TestData.ResponseMessage,
                                                                                        ResponseCode = TestData.ResponseCode,
                                                                                        EstateId = TestData.EstateId,
                                                                                        MerchantId = TestData.MerchantId
                                                                                    };

        public static SerialisedMessage SerialisedMessageResponseReconciliation = new SerialisedMessage
                                                                                  {
                                                                                      Metadata = new Dictionary<String, String>
                                                                                                 {
                                                                                                     {"EstateId", TestData.EstateId.ToString()},
                                                                                                     {"MerchantId", TestData.MerchantId.ToString()},
                                                                                                 },
                                                                                      SerialisedData = JsonConvert.SerializeObject(TestData.ClientReconciliationResponse, new JsonSerializerSettings
                                                                                          {
                                                                                              TypeNameHandling = TypeNameHandling.All
                                                                                          })
                                                                                  };

        public static IReadOnlyDictionary<String, String> DefaultAppSettings =>
            new Dictionary<String, String>
            {
                ["AppSettings:MinimumSupportedApplicationVersion"] = "1.0.5",
                ["AppSettings:SecurityService"] = "http://192.168.1.133:5001",
                ["AppSettings:TransactionProcessorApi"]  = "http://192.168.1.133:5002",
                ["AppSettings:ClientId"] = "ClientId",
                ["AppSettings:ClientSecret"] = "secret",
                ["SecurityConfiguration:Authority"] = "https://127.0.0.1",
                ["EventStoreSettings:ConnectionString"] = "https://127.0.0.1:2113"
            };

        public static IReadOnlyDictionary<String, String> DefaultAppSettingsSkipVersionCheck =>
            new Dictionary<String, String>
            {
                ["AppSettings:MinimumSupportedApplicationVersion"] = "1.0.5",
                ["AppSettings:SecurityService"] = "http://192.168.1.133:5001",
                ["AppSettings:TransactionProcessorApi"] = "http://192.168.1.133:5002",
                ["AppSettings:ClientId"] = "ClientId",
                ["AppSettings:ClientSecret"] = "secret",
                ["SecurityConfiguration:Authority"] = "https://127.0.0.1",
                ["EventStoreSettings:ConnectionString"] = "https://127.0.0.1:2113",
                ["AppSettings:SkipVersionCheck"] = "true"
            };

        public static String OldApplicationVersion = "1.0.4";
        public static String NewerApplicationVersion = "1.0.5.1";
        public static String ApplicationVersion = "1.0.5";

        public static VersionCheckCommands.VersionCheckCommand VersionCheckCommand = new(TestData.ApplicationVersion);
        public static VoucherCommands.RedeemVoucherCommand RedeemVoucherCommand => new(TestData.EstateId, TestData.ContractId, TestData.VoucherCode);
        public static MerchantQueries.GetMerchantContractsQuery GetMerchantContractsQuery => new(EstateId,MerchantId);
        public static MerchantQueries.GetMerchantQuery GetMerchantQuery => new(EstateId, MerchantId);

        public static VoucherQueries.GetVoucherQuery GetVoucherQuery => new(TestData.EstateId, TestData.ContractId, TestData.VoucherCode);

        public static String VoucherCode = "1231231234";
        public static DateTime ExpiryDate = new DateTime(2021, 1, 11);
        public static Decimal Value = 20.00m;
        public static Guid VoucherId = Guid.Parse("C12665AC-1301-47AA-B292-281EC4DE9721");
        public static Decimal Balance = 10.00m;
        public static Models.GetVoucherResponse GetVoucherResponseModel =>
            new Models.GetVoucherResponse
            {
                VoucherCode = TestData.VoucherCode,
                ExpiryDate = TestData.ExpiryDate,
                ContractId = TestData.ContractId,
                Value = TestData.Value,
                VoucherId = TestData.VoucherId,
                EstateId = TestData.EstateId,
                ResponseCode = TestData.ResponseCode,
                ResponseMessage = TestData.ResponseMessage,
                Balance = TestData.Balance
            };

        public static Models.RedeemVoucherResponse RedeemVoucherResponseModel =>
            new Models.RedeemVoucherResponse
            {
                VoucherCode = TestData.VoucherCode,
                ExpiryDate = TestData.ExpiryDate,
                ContractId = TestData.ContractId,
                Balance = TestData.Balance,
                EstateId = TestData.EstateId,
                ResponseCode = TestData.ResponseCode,
                ResponseMessage = TestData.ResponseMessage
            };

        public static GetVoucherResponse GetVoucherResponse =>
            new GetVoucherResponse
            {
                VoucherCode = TestData.VoucherCode,
                Balance = TestData.Balance,
                ExpiryDate = TestData.ExpiryDate,
                GeneratedDateTime = TestData.GeneratedDateTime,
                IsGenerated = TestData.IsGenerated,
                IsIssued = TestData.IsIssued,
                IsRedeemed = TestData.IsRedeemed,
                IssuedDateTime = TestData.IssuedDateTime,
                RedeemedDateTime = TestData.RedeemedDateTime,
                TransactionId = TestData.TransactionId,
                Value = TestData.Value,
                VoucherId = TestData.VoucherId
            };

        public static RedeemVoucherResponse RedeemVoucherResponse =>
            new RedeemVoucherResponse
            {
                VoucherCode = TestData.VoucherCode,
                RemainingBalance = TestData.Balance,
                ExpiryDate = TestData.ExpiryDate
            };

        public static DateTime GeneratedDateTime = new DateTime(2020, 12, 11);

        public static Boolean IsGenerated = true;

        public static Boolean IsIssued = true;

        public static Boolean IsRedeemed = false;

        public static DateTime IssuedDateTime = new DateTime(2020, 12, 12);

        public static DateTime RedeemedDateTime = new DateTime();

        public static Guid TransactionId = Guid.Parse("793ACA88-B501-435E-BF08-1E5F639A7885");
        public static String MerchantName = "Test Merchant Name";
        public static Guid DeviceId = Guid.Parse("840F32FF-8B74-467C-8078-F5D9297FED56");
        public static String MerchantNumber = "12345678";
        public static String AddressLine1 = "AddressLine1";
        public static String AddressLine2 = "AddressLine2";
        public static String AddressLine3 = "AddressLine3";
        public static String AddressLine4 = "AddressLine4";
        public static String Country = "Country";
        public static String PostCode = "PostCode";
        public static String Region = "Region";
        public static String Town = "Town";
        public static TransactionProcessor.DataTransferObjects.Responses.Merchant.AddressResponse Address =>
            new TransactionProcessor.DataTransferObjects.Responses.Merchant.AddressResponse
            {
                AddressLine1 = TestData.AddressLine1,
                AddressLine2 = TestData.AddressLine2,
                AddressLine3 = TestData.AddressLine3,
                AddressLine4 = TestData.AddressLine4,
                Country = TestData.Country,
                PostalCode = TestData.PostCode,
                Region = TestData.Region,
                Town = TestData.Town
            };

        public static String ContactName = "Test Contact";
        public static String ContactPhone = "123456789";
        public static String ContactEmail = "testcontact1@testmerchant1.co.uk";
        public static TransactionProcessor.DataTransferObjects.Responses.Contract.ContactResponse Contact =>
            new ()
            {
                ContactName = TestData.ContactName,
                ContactEmailAddress = TestData.ContactEmail,
                ContactPhoneNumber = TestData.ContactPhone
            };
        public static String TerminalNumber = "00000001";
        public static TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantResponse MerchantResponse(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule  settlementSchedule) =>
            new()
            {
                EstateId = TestData.EstateId,
                MerchantId = TestData.MerchantId,
                MerchantName = TestData.MerchantName,
                Devices = new Dictionary<Guid, String>
                {
                    {TestData.DeviceId, TestData.DeviceIdentifier}
                },
                Operators = new()
                {
                    new()
                    {
                        OperatorId = TestData.OperatorId,
                        MerchantNumber = TestData.MerchantNumber,
                        TerminalNumber = TestData.TerminalNumber
                    }
                },
                Contracts = new(){
                    new (){
                        ContractId = TestData.ContractId,
                        ContractProducts = new List<Guid>(){
                            TestData.ProductId
                        }
                    }
                },
                SettlementSchedule = settlementSchedule,
                Addresses = new (){
                    Address
                },
                Contacts = new (){
                    Contact
                },
            };
        public static List<TransactionProcessor.DataTransferObjects.Responses.Contract.ContractResponse> MerchantContractResponses(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType productType) =>
            new() {
                new() {
                    ContractId = TestData.ContractId,
                    Products = new() {
                        new () {
                            ProductId = TestData.ProductId,
                            ProductType = productType
                        }
                    }
                }
            };
    }
}