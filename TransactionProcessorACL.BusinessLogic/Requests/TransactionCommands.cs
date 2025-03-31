using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using SimpleResults;
using TransactionProcessor.DataTransferObjects.Responses.Merchant;
using TransactionProcessorACL.Models;
using MerchantResponse = TransactionProcessorACL.Models.MerchantResponse;

namespace TransactionProcessorACL.BusinessLogic.Requests;

[ExcludeFromCodeCoverage]
public record TransactionCommands {
    public record ProcessLogonTransactionCommand(Guid EstateId,
                                                 Guid MerchantId,
                                                 DateTime TransactionDateTime,
                                                 String TransactionNumber,
                                                 String DeviceIdentifier)
        : IRequest<Result<ProcessLogonTransactionResponse>>;

    public record ProcessSaleTransactionCommand(Guid EstateId,
                                                Guid MerchantId,
                                                DateTime TransactionDateTime,
                                                String TransactionNumber,
                                                String DeviceIdentifier,
                                                Guid OperatorId,
                                                String CustomerEmailAddress,
                                                Guid ContractId,
                                                Guid ProductId,
                                                Dictionary<String, String> AdditionalRequestMetadata)
        : IRequest<Result<ProcessSaleTransactionResponse>>;

    public record ProcessReconciliationCommand(Guid EstateId,
                                               Guid MerchantId,
                                               DateTime TransactionDateTime,
                                               String DeviceIdentifier,
                                               Int32 TransactionCount,
                                               Decimal TransactionValue)
        : IRequest<Result<ProcessReconciliationResponse>>;
}

[ExcludeFromCodeCoverage]
public record MerchantQueries {
    public record GetMerchantContractsQuery(Guid EstateId,Guid MerchantId) : IRequest<Result<List<ContractResponse>>>;
    public record GetMerchantQuery(Guid EstateId, Guid MerchantId) : IRequest<Result<MerchantResponse>>;

}