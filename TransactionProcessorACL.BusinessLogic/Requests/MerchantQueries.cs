using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.Requests;

[ExcludeFromCodeCoverage]
public record MerchantQueries {
    public record GetMerchantContractsQuery(Guid EstateId,Guid MerchantId) : IRequest<Result<List<ContractResponse>>>;
    public record GetMerchantQuery(Guid EstateId, Guid MerchantId) : IRequest<Result<MerchantResponse>>;
    public record GetMerchantScheduleQuery(Guid EstateId, Guid MerchantId, Int32 Year) : IRequest<Result<MerchantScheduleResponse>>;

}