using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shared.Results;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects;

namespace TransactionProcessorACL.BusinessLogic.BackendAPI;

public class EstateReportingApiClient : ClientProxyBase.ClientProxyBase, IEstateReportingApiClient
{
    private readonly Func<string, string> BaseAddressResolver;
    private const string EstateIdHeaderName = "EstateId";

    public EstateReportingApiClient(Func<string, string> baseAddressResolver,
                                    HttpClient httpClient,
                                    Func<object, string> serialise,
                                    Func<string, Type, object> deserialise) : base(httpClient, serialise, deserialise)
    {
        this.BaseAddressResolver = baseAddressResolver;
    }

    public async Task<Result<MerchantDailyPerformanceSummaryResponse>> GetMerchantDailyPerformanceSummary(String accessToken,
                                                                                                          Guid estateId,
                                                                                                          MerchantDailyPerformanceSummaryRequest request,
                                                                                                          CancellationToken cancellationToken)
    {
        string requestUri = this.BuildRequestUrl("/api/transactions/merchantdailyperformancesummary");

        try
        {
            List<(string headerName, string headerValue)> additionalHeaders =
            [
                (EstateIdHeaderName, estateId.ToString())
            ];

            Result<MerchantDailyPerformanceSummaryResponse> result =
                await this.Post<MerchantDailyPerformanceSummaryRequest, MerchantDailyPerformanceSummaryResponse>(requestUri, request, accessToken, additionalHeaders, cancellationToken);

            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result);

            return result;
        }
        catch (Exception ex)
        {
            Exception exception = new($"Error getting merchant daily performance summary for estate {estateId}.", ex);
            return Result.Failure(exception.Message);
        }
    }

    public async Task<Result<TransactionMixSummaryResponse>> GetMerchantTransactionMixSummary(String accessToken,
                                                                                              Guid estateId,
                                                                                              TransactionMixSummaryRequest request,
                                                                                              CancellationToken cancellationToken)
    {
        string requestUri = this.BuildRequestUrl("/api/transactions/transactionmixsummary");

        try
        {
            List<(string headerName, string headerValue)> additionalHeaders =
            [
                (EstateIdHeaderName, estateId.ToString())
            ];

            Result<TransactionMixSummaryResponse> result =
                await this.Post<TransactionMixSummaryRequest, TransactionMixSummaryResponse>(requestUri, request, accessToken, additionalHeaders, cancellationToken);

            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result);

            return result;
        }
        catch (Exception ex)
        {
            Exception exception = new($"Error getting merchant transaction mix summary for estate {estateId}.", ex);
            return Result.Failure(exception.Message);
        }
    }

    private string BuildRequestUrl(string relativePath)
    {
        string baseAddress = this.BaseAddressResolver("EstateReportingApi");
        return $"{baseAddress.TrimEnd('/')}{relativePath}";
    }
}
