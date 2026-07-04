using TransactionProcessor.IntegrationTesting.Helpers;
using TransactionProcessorACL.DataTransferObjects.Responses;
using TransactionProcessorACL.IntegrationTests.Shared;

namespace TransactionProcessor.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EstateDetails1
    {
        public readonly EstateDetails EstateDetails;

        private readonly Dictionary<Guid, Dictionary<String, String>> MerchantUsersTokens;
        private readonly Dictionary<Guid, Int32> MerchantReportingIds;
        private readonly Dictionary<Guid, MerchantDailyPerformanceSummaryResponse> MerchantDailyPerformanceSummaryResponses;
        private readonly Dictionary<Guid, MerchantTransactionMixSummaryResponse> MerchantTransactionMixSummaryResponses;
        private Dictionary<String, Dictionary<String, String>> VoucherRedemptionUsersTokens;

        public EstateDetails1(EstateDetails estateDetails)
        {
            this.EstateDetails = estateDetails;
            this.MerchantUsersTokens = new Dictionary<Guid, Dictionary<String, String>>();
            this.MerchantReportingIds = new Dictionary<Guid, Int32>();
            this.MerchantDailyPerformanceSummaryResponses = new Dictionary<Guid, MerchantDailyPerformanceSummaryResponse>();
            this.MerchantTransactionMixSummaryResponses = new Dictionary<Guid, MerchantTransactionMixSummaryResponse>();
            this.VoucherRedemptionUsersTokens = new Dictionary<String, Dictionary<String, String>>();
        }

        public void AddMerchantUserToken(Guid merchantId,
                                         String userName,
                                         String token)
        {
            if (this.MerchantUsersTokens.ContainsKey(merchantId))
            {
                Dictionary<String, String> merchantUsersList = this.MerchantUsersTokens[merchantId];
                if (merchantUsersList.ContainsKey(userName) == false)
                {
                    merchantUsersList.Add(userName, token);
                }
            }
            else
            {
                Dictionary<String, String> merchantUsersList = new Dictionary<String, String>();
                merchantUsersList.Add(userName, token);
                this.MerchantUsersTokens.Add(merchantId, merchantUsersList);
            }
        }

        public void AddMerchantReportingId(Guid merchantId,
                                           Int32 merchantReportingId)
        {
            if (this.MerchantReportingIds.ContainsKey(merchantId))
            {
                this.MerchantReportingIds[merchantId] = merchantReportingId;
            }
            else
            {
                this.MerchantReportingIds.Add(merchantId, merchantReportingId);
            }
        }

        public void AddMerchantDailyPerformanceSummaryResponse(Guid merchantId,
                                                               MerchantDailyPerformanceSummaryResponse response)
        {
            if (this.MerchantDailyPerformanceSummaryResponses.ContainsKey(merchantId))
            {
                this.MerchantDailyPerformanceSummaryResponses[merchantId] = response;
            }
            else
            {
                this.MerchantDailyPerformanceSummaryResponses.Add(merchantId, response);
            }
        }

        public void AddMerchantTransactionMixSummaryResponse(Guid merchantId,
                                                             MerchantTransactionMixSummaryResponse response)
        {
            if (this.MerchantTransactionMixSummaryResponses.ContainsKey(merchantId))
            {
                this.MerchantTransactionMixSummaryResponses[merchantId] = response;
            }
            else
            {
                this.MerchantTransactionMixSummaryResponses.Add(merchantId, response);
            }
        }

        public void AddVoucherRedemptionUserToken(String operatorName,
                                                  String userName,
                                                  String token)
        {
            if (this.VoucherRedemptionUsersTokens.ContainsKey(operatorName))
            {
                Dictionary<String, String> merchantUsersList = this.VoucherRedemptionUsersTokens[operatorName];
                if (merchantUsersList.ContainsKey(userName) == false)
                {
                    merchantUsersList.Add(userName, token);
                }
            }
            else
            {
                Dictionary<String, String> merchantUsersList = new Dictionary<String, String>();
                merchantUsersList.Add(userName, token);
                this.VoucherRedemptionUsersTokens.Add(operatorName, merchantUsersList);
            }
        }

        public String GetMerchantUserToken(Guid merchantId)
        {
            KeyValuePair<Guid, Dictionary<String, String>> x = this.MerchantUsersTokens.SingleOrDefault(x => x.Key == merchantId);

            if (x.Value != null)
            {
                return x.Value.First().Value;
            }

            return string.Empty;
        }

        public Int32 GetMerchantReportingId(Guid merchantId)
        {
            return this.MerchantReportingIds.Single(x => x.Key == merchantId).Value;
        }

        public MerchantDailyPerformanceSummaryResponse GetMerchantDailyPerformanceSummaryResponse(Guid merchantId)
        {
            return this.MerchantDailyPerformanceSummaryResponses.SingleOrDefault(x => x.Key == merchantId).Value;
        }

        public MerchantTransactionMixSummaryResponse GetMerchantTransactionMixSummaryResponse(Guid merchantId)
        {
            return this.MerchantTransactionMixSummaryResponses.SingleOrDefault(x => x.Key == merchantId).Value;
        }

        public String GetVoucherRedemptionUserToken(String operatorName)
        {
            KeyValuePair<String, Dictionary<String, String>> x = this.VoucherRedemptionUsersTokens.SingleOrDefault(x => x.Key == operatorName);

            if (x.Value != null)
            {
                return x.Value.First().Value;
            }

            return string.Empty;
        }
    }
}
