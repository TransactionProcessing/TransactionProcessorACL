using System;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.Requests;

public record VoucherQueries {
    public record GetVoucherQuery(Guid EstateId, Guid ContractId, String VoucherCode)
        : IRequest<Result<GetVoucherResponse>>;
}