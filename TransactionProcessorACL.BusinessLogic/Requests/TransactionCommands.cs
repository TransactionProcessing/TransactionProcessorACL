using System;
using System.Collections.Generic;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.Requests;

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