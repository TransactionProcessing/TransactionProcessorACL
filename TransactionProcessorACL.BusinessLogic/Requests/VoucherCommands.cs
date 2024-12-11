using System;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.Requests;

[ExcludeFromCodeCoverage]
public record VoucherCommands {
    public record RedeemVoucherCommand(Guid EstateId, Guid ContractId, String VoucherCode) : IRequest<Result<RedeemVoucherResponse>>;
}