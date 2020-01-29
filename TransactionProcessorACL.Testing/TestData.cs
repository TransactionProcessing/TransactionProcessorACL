namespace TransactionProcessorACL.Testing
{
    using System;
    using System.Collections.Generic;
    using BusinessLogic.Requests;
    using DataTransferObjects;
    using Models;
    using Newtonsoft.Json;
    using SecurityService.DataTransferObjects.Responses;
    using TransactionProcessor.DataTransferObjects;

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
        /// The imei number
        /// </summary>
        public static String DeviceIdentifier = "12345678";

        /// <summary>
        /// The require configuration in response true
        /// </summary>
        public static Boolean RequireConfigurationInResponseTrue = true;

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
                                                                                          RequireConfigurationInResponse = TestData.RequireConfigurationInResponseTrue,
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
        public static ProcessLogonTransactionRequest ProcessLogonTransactionRequest =
            ProcessLogonTransactionRequest.Create(TestData.EstateId,
                                                  TestData.MerchantId,
                                                  TestData.TransactionDateTime,
                                                  TestData.TransactionNumber,
                                                  TestData.DeviceIdentifier,
                                                  TestData.RequireConfigurationInResponseTrue);

        public static String ResponseCode = "0000";

        public static String ResponseMessage = "SUCCESS";

        public static ProcessLogonTransactionResponse ProcessLogonTransactionResponse = new ProcessLogonTransactionResponse
                                                                                        {
                                                                                            ResponseMessage = TestData.ResponseMessage,
                                                                                            ResponseCode = TestData.ResponseCode
                                                                                        };

        public static SerialisedMessage SerialisedMessageResponse = new SerialisedMessage
                                                                    {
                                                                        Metadata = new Dictionary<String, String>
                                                                                   {
                                                                                       {"EstateId", TestData.EstateId.ToString()},
                                                                                       {"MerchantId", TestData.MerchantId.ToString()},
                                                                                   },
                                                                        SerialisedData = JsonConvert.SerializeObject(TestData.ProcessLogonTransactionResponse, new JsonSerializerSettings
                                                                                                                                                               {
                                                                                                                                                                   TypeNameHandling = TypeNameHandling.All
                                                                                                                                                               })
                                                                    };

        #endregion

        public static String Token = "{\"access_token\": \"eyJhbGciOiJSUzI1NiIsImtpZCI6IjA4NGZlNTIwZmIzZmVhM2M0MmNmMjBiZWM2OGY1NDg2IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE1Nzc1NTIyMTQsImV4cCI6MTU3NzU1NTgxNCwiaXNzIjoiaHR0cDovLzE5Mi4xNjguMS4xMzM6NTAwMSIsImF1ZCI6WyJlc3RhdGVNYW5hZ2VtZW50IiwidHJhbnNhY3Rpb25Qcm9jZXNzb3IiLCJ0cmFuc2FjdGlvblByb2Nlc3NvckFDTCJdLCJjbGllbnRfaWQiOiJzZXJ2aWNlQ2xpZW50Iiwic2NvcGUiOlsiZXN0YXRlTWFuYWdlbWVudCIsInRyYW5zYWN0aW9uUHJvY2Vzc29yIiwidHJhbnNhY3Rpb25Qcm9jZXNzb3JBQ0wiXX0.JxK6kEvmvuMnL7ktgvv6N52fjqnFG-NSjPcQORIcFb4ravZAk5oNgsnEtjPcOHTXiktcr8i361GlYO1yiSGdfLKtCTaH3lihcbOb1wfQh3bYM_xmlqJUdirerR8ql9lxqBqm2_Q__PDFuFhMd9lAz-iFr3svuOXeQJB5HQ2rtA3xBDDked5SaEs1dMfbBJA6svRq831WCQSJgap2Db7XN7ais7AQhPYUcFOTGs9Qk33rF_k-2dnAzkEITjvgPwim-8YsEQfsbRYgZmIXfjOXcD81Y0G2_grugvc0SOj_hKXd4d62T-zU-mC4opuYauWKYFqV4UB4sf4V4rtLWeDWrA\",\"expires_in\": 3600,\"token_type\": \"Bearer\",\"scope\": \"estateManagement transactionProcessor transactionProcessorACL\"}";

        public static TokenResponse TokenResponse = TokenResponse.Create(TestData.Token);
        
        
    }
}