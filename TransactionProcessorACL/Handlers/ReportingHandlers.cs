using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.General;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects.Requests;
using ApiResponses = TransactionProcessorACL.DataTransferObjects.Responses;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers;

public static class ReportingHandlers
{
    public static async Task<IResult> GetMerchantDailyPerformanceSummary(ClaimsPrincipal user,
                                                                         MerchantDailyPerformanceSummaryRequest request,
                                                                         IMediator mediator,
                                                                         CancellationToken cancellationToken)
    {
        Result<System.Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        ReportingQueries.GetMerchantDailyPerformanceSummaryQuery query = new(estateIdResult.Data, request);
        Result<MerchantDailyPerformanceSummaryResponse> result =
            await mediator.Send(query, cancellationToken);
        return ResponseFactory.FromResult(result, response => new ApiResponses.MerchantDailyPerformanceSummaryResponse
        {
            Metrics = response.Metrics.Select(metric => new ApiResponses.MetricItem
            {
                Category = metric.Category,
                Description = metric.Description,
                Title = metric.Title,
                Value = metric.Value
            }).ToList(),
            DrillDownTransactions = response.DrillDownTransactions.Select(transaction => new ApiResponses.DrillDownTransaction
            {
                Amount = transaction.Amount,
                Product = transaction.Product,
                Reference = transaction.Reference,
                Status = transaction.Status,
                TransactionDateTime = transaction.TransactionDateTime
            }).ToList()
        });
    }

    public static async Task<IResult> GetMerchantTransactionMixSummary(ClaimsPrincipal user,
                                                                       MerchantTransactionMixSummaryRequest request,
                                                                       IMediator mediator,
                                                                       CancellationToken cancellationToken)
    {
        Result<System.Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        ReportingQueries.GetMerchantTransactionMixSummaryQuery query = new(estateIdResult.Data, request);
        Result<MerchantTransactionMixSummaryResponse> result = await mediator.Send(query, cancellationToken);
        return ResponseFactory.FromResult(result, response => new ApiResponses.MerchantTransactionMixSummaryResponse
        {
            Breakdown = Enum.Parse<TransactionProcessorACL.DataTransferObjects.Requests.TransactionMixBreakdown>(response.Breakdown.ToString()),
            Measure = Enum.Parse<TransactionProcessorACL.DataTransferObjects.Requests.TransactionMixMeasure>(response.Measure.ToString()),
            FromDate = response.FromDate,
            ToDate = response.ToDate,
            TotalCount = response.TotalCount,
            TotalValue = response.TotalValue,
            Items = response.Items.Select(item => new ApiResponses.TransactionMixSummaryItem
            {
                Key = item.Key,
                Label = item.Label,
                Count = item.Count,
                Value = item.Value
            }).ToList(),
            DrillDownTransactions = response.DrillDownTransactions.Select(transaction => new ApiResponses.TransactionMixDrillDownTransaction
            {
                Amount = transaction.Amount,
                Operator = transaction.Operator,
                Product = transaction.Product,
                Reference = transaction.Reference,
                Status = transaction.Status,
                TransactionDateTime = transaction.TransactionDateTime,
                TransactionType = transaction.TransactionType
            }).ToList()
        });
    }

    public static async Task<IResult> GetRecentActivityReceiptSearch(ClaimsPrincipal user,
                                                                     RecentActivityReceiptSearchRequest request,
                                                                     IMediator mediator,
                                                                     CancellationToken cancellationToken)
    {
        Result<System.Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        ReportingQueries.GetRecentActivityReceiptSearchQuery query = new(estateIdResult.Data, request);
        Result<RecentActivityReceiptSearchResponse> result =
            await mediator.Send(query, cancellationToken);
        return ResponseFactory.FromResult(result, response => new ApiResponses.RecentActivityReceiptSearchResponse
        {
            ReportDate = response.ReportDate,
            PageNumber = response.PageNumber,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(item => new ApiResponses.RecentActivityReceiptSearchItem
            {
                Amount = item.Amount,
                Operator = item.Operator,
                Product = item.Product,
                ReceiptReference = item.ReceiptReference,
                Reference = item.Reference,
                Status = item.Status,
                TransactionDateTime = item.TransactionDateTime,
                TransactionType = item.TransactionType
            }).ToList()
        });
    }
}
