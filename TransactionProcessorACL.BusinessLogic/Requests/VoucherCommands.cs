using System;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.Requests;

public record VoucherCommands {
    public record RedeemVoucherCommand(Guid EstateId, Guid ContractId, String VoucherCode) : IRequest<Result<RedeemVoucherResponse>>;
}