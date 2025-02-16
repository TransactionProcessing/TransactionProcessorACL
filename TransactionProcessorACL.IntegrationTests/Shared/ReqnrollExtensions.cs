using TransactionProcessor.IntegrationTesting.Helpers;

namespace TransactionProcessorACL.IntegrationTests.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using DataTransferObjects;
using global::Shared.IntegrationTesting;
using Newtonsoft.Json;
using Reqnroll;
using SecurityService.DataTransferObjects;
using Shouldly;
using TransactionProcessor.DataTransferObjects;
using TransactionProcessor.IntegrationTests.Common;
using OperatorTotalRequest = DataTransferObjects.OperatorTotalRequest;

public static class ReqnrollExtensions
{
    public static List<CreateUserRequest> ToAclCreateUserRequests(this DataTableRows tableRows, List<EstateDetails1> estateDetailsList){
        List<CreateUserRequest> requests = new List<CreateUserRequest>();

        foreach (DataTableRow tableRow in tableRows)
        {
            Dictionary<String, String> userClaims = new Dictionary<String, String>();
            String estateName = ReqnrollTableHelper.GetStringRowValue(tableRow, "EstateName");
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
            es1.ShouldNotBeNull();
            userClaims.Add("estateId", es1.EstateDetails.EstateId.ToString());
            userClaims.Add("contractId", es1.EstateDetails.EstateId.ToString());

            String roles = ReqnrollTableHelper.GetStringRowValue(tableRow, "Roles");

            CreateUserRequest createUserRequest = new CreateUserRequest
                                                  {
                                                      EmailAddress = ReqnrollTableHelper.GetStringRowValue(tableRow, "Email Address"),
                                                      FamilyName = ReqnrollTableHelper.GetStringRowValue(tableRow, "Family Name"),
                                                      GivenName = ReqnrollTableHelper.GetStringRowValue(tableRow, "Given Name"),
                                                      PhoneNumber = ReqnrollTableHelper.GetStringRowValue(tableRow, "Phone Number"),
                                                      MiddleName = ReqnrollTableHelper.GetStringRowValue(tableRow, "Middle name"),
                                                      Claims = userClaims,
                                                      Roles = string.IsNullOrEmpty(roles) ? null : roles.Split(",").ToList(),
                                                      Password = ReqnrollTableHelper.GetStringRowValue(tableRow, "Password")
                                                  };

            requests.Add(createUserRequest);
        }

        return requests;
    }

    public static List<(EstateDetails,String, Guid, String, TransactionRequestMessage)> ToACLSerialisedMessages(String applicationVersion, List<(EstateDetails, Guid, String, SerialisedMessage)> serialisedMessages, List<EstateDetails1> estateDetailsList){
        List<(EstateDetails,String, Guid, String,TransactionRequestMessage)> aclRequestMessages = new List<(EstateDetails,String, Guid, String, TransactionRequestMessage)>();
        foreach ((EstateDetails, Guid, String, SerialisedMessage) sm in serialisedMessages){
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateId == sm.Item1.EstateId);
            es1.ShouldNotBeNull();
            String merchantToken = es1.GetMerchantUserToken(sm.Item2);
                
            Object x = JsonConvert.DeserializeObject(sm.Item4.SerialisedData, new JsonSerializerSettings{
                                                                                                            TypeNameHandling = TypeNameHandling.All
                                                                                                        });
            TransactionRequestMessage transactionRequest = null;
            if (x.GetType() == typeof(LogonTransactionRequest)){
                LogonTransactionRequest logonRequest = (LogonTransactionRequest)x;
                transactionRequest = new LogonTransactionRequestMessage{
                                                                           ApplicationVersion = applicationVersion,
                                                                           DeviceIdentifier = logonRequest.DeviceIdentifier,
                                                                           TransactionDateTime = logonRequest.TransactionDateTime,
                                                                           TransactionNumber = logonRequest.TransactionNumber
                                                                       };

            }

            if (x.GetType() == typeof(SaleTransactionRequest)){
                SaleTransactionRequest saleRequest = (SaleTransactionRequest)x;
                transactionRequest = new SaleTransactionRequestMessage{
                                                                          TransactionNumber = saleRequest.TransactionNumber,
                                                                          ApplicationVersion = applicationVersion,
                                                                          DeviceIdentifier = saleRequest.DeviceIdentifier,
                                                                          TransactionDateTime = saleRequest.TransactionDateTime,
                                                                          AdditionalRequestMetaData = saleRequest.AdditionalTransactionMetadata,
                                                                          ContractId = saleRequest.ContractId,
                                                                          CustomerEmailAddress = saleRequest.CustomerEmailAddress,
                                                                          OperatorId = saleRequest.OperatorId,
                                                                          ProductId = saleRequest.ProductId,
                                                                      };
            }

            if (x.GetType() == typeof(ReconciliationRequest)){
                ReconciliationRequest reconciliation = (ReconciliationRequest)x;
                transactionRequest = new ReconciliationRequestMessage{
                                                                         TransactionDateTime = reconciliation.TransactionDateTime,
                                                                         DeviceIdentifier = reconciliation.DeviceIdentifier,
                                                                         TransactionValue = reconciliation.TransactionValue,
                                                                         TransactionCount = reconciliation.TransactionCount,
                                                                         ApplicationVersion = applicationVersion,
                                                                         TransactionNumber = sm.Item3
                                                                     };

                if (reconciliation.OperatorTotals != null){
                    // This is nasty so very sorry...
                    ((ReconciliationRequestMessage)transactionRequest).OperatorTotals = reconciliation.OperatorTotals.Select(o => new OperatorTotalRequest

                                                                                                                                  {
                                                                                                                                      ContractId = o.ContractId,
                                                                                                                                      OperatorId = o.OperatorId,
                                                                                                                                      TransactionCount = o.TransactionCount,
                                                                                                                                      TransactionValue = o.TransactionValue,
                                                                                                                                  }).ToList();

                }

            }

            aclRequestMessages.Add((es1.EstateDetails, merchantToken, sm.Item2, sm.Item3, transactionRequest));

        }

        return aclRequestMessages;
    }

    public static List<ExpectedTransactionResponse> ToExpectedTransactionResponseDetails(this DataTableRows tableRows, List<EstateDetails1> estateDetailsList)
    {
        List<ExpectedTransactionResponse> expectedTransactionResponses = new List<ExpectedTransactionResponse>();
        List<(String responseCode, String responseMessage)> responseMessages = new List<(String responseCode, String responseMessage)>();
        foreach (DataTableRow tableRow in tableRows){
            ExpectedTransactionResponse expectedTransactionResponse = new ExpectedTransactionResponse{
                                                                                                         EstateName = ReqnrollTableHelper.GetStringRowValue(tableRow, "EstateName"),
                                                                                                         TransactionNumber = ReqnrollTableHelper.GetStringRowValue(tableRow, "TransactionNumber"),
                                                                                                         MerchantName = ReqnrollTableHelper.GetStringRowValue(tableRow, "MerchantName"),
                                                                                                         ResponseCode = ReqnrollTableHelper.GetStringRowValue(tableRow, "ResponseCode"),
                                                                                                         ResponseMessage = ReqnrollTableHelper.GetStringRowValue(tableRow, "ResponseMessage"),
                                                                                                         TransactionType = ReqnrollTableHelper.GetStringRowValue(tableRow, "TransactionType")
                                                                                                     };
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
            es1.ShouldNotBeNull();
            expectedTransactionResponse.MerchantId = es1.EstateDetails.GetMerchantId(expectedTransactionResponse.MerchantName);
            expectedTransactionResponses.Add(expectedTransactionResponse);
        }
        return expectedTransactionResponses;
    }

    public static List<ExpectedReconciliationResponse> ToExpectedReconciliationResponseDetails(this DataTableRows tableRows, List<EstateDetails1> estateDetailsList)
    {
        List<ExpectedReconciliationResponse> expectedReconciliationResponses = new List<ExpectedReconciliationResponse>();
        List<(String responseCode, String responseMessage)> responseMessages = new List<(String responseCode, String responseMessage)>();
        foreach (DataTableRow tableRow in tableRows)
        {
            ExpectedReconciliationResponse expectedTransactionResponse = new ExpectedReconciliationResponse
                                                                         {
                                                                             EstateName = ReqnrollTableHelper.GetStringRowValue(tableRow, "EstateName"),
                                                                             MerchantName = ReqnrollTableHelper.GetStringRowValue(tableRow, "MerchantName"),
                                                                             ResponseCode = ReqnrollTableHelper.GetStringRowValue(tableRow, "ResponseCode"),
                                                                             ResponseMessage = ReqnrollTableHelper.GetStringRowValue(tableRow, "ResponseMessage"),
                                                                             TransactionNumber = ReqnrollTableHelper.GetStringRowValue(tableRow, "TransactionNumber"),
                                                                         };
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
            es1.ShouldNotBeNull();
            expectedTransactionResponse.MerchantId = es1.EstateDetails.GetMerchantId(expectedTransactionResponse.MerchantName);
            expectedReconciliationResponses.Add(expectedTransactionResponse);
        }
        return expectedReconciliationResponses;
    }

    public class ExpectedTransactionResponse{
        public String ResponseCode{ get; set; }
        public String ResponseMessage { get; set; }
        public Guid MerchantId { get; set; }
        public String EstateName { get; set; }
        public String MerchantName { get; set; }
        public String TransactionNumber { get; set; }
        public String TransactionType { get; set; }
    }

    public class ExpectedReconciliationResponse
    {
        public String ResponseCode { get; set; }
        public String ResponseMessage { get; set; }
        public Guid MerchantId { get; set; }
        public String EstateName { get; set; }
        public String MerchantName { get; set; }
        public String TransactionNumber { get; set; }
    }
}