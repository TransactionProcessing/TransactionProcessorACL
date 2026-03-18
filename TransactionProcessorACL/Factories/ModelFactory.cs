namespace TransactionProcessorACL.Factories
{
    using DataTransferObjects.Responses;
    using Models;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.Factories.IModelFactory" />
    public class ModelFactory : IModelFactory
    {
        #region Methods

        public LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse)
        {
            if (processLogonTransactionResponse == null)
            {
                return null;
            }

            LogonTransactionResponseMessage logonTransactionResponseMessage = new LogonTransactionResponseMessage();

            logonTransactionResponseMessage.ResponseMessage = processLogonTransactionResponse.ResponseMessage;
            logonTransactionResponseMessage.ResponseCode = processLogonTransactionResponse.ResponseCode;
            logonTransactionResponseMessage.MerchantId = processLogonTransactionResponse.MerchantId;
            logonTransactionResponseMessage.EstateId = processLogonTransactionResponse.EstateId;
            logonTransactionResponseMessage.TransactionId = processLogonTransactionResponse.TransactionId;

            return logonTransactionResponseMessage;
        }

        public SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse)
        {
            if (processSaleTransactionResponse == null)
            {
                return null;
            }

            SaleTransactionResponseMessage saleTransactionResponseMessage = new SaleTransactionResponseMessage();

            saleTransactionResponseMessage.ResponseMessage = processSaleTransactionResponse.ResponseMessage;
            saleTransactionResponseMessage.ResponseCode = processSaleTransactionResponse.ResponseCode;
            saleTransactionResponseMessage.MerchantId = processSaleTransactionResponse.MerchantId;
            saleTransactionResponseMessage.EstateId = processSaleTransactionResponse.EstateId;
            saleTransactionResponseMessage.TransactionId = processSaleTransactionResponse.TransactionId;
            saleTransactionResponseMessage.AdditionalResponseMetadata = processSaleTransactionResponse.AdditionalTransactionMetadata;
            

            return saleTransactionResponseMessage;
        }

        public ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse)
        {
            if (processReconciliationResponse == null)
            {
                return null;
            }

            ReconciliationResponseMessage reconciliationResponseMessage = new ReconciliationResponseMessage();

            reconciliationResponseMessage.ResponseMessage = processReconciliationResponse.ResponseMessage;
            reconciliationResponseMessage.ResponseCode = processReconciliationResponse.ResponseCode;
            reconciliationResponseMessage.MerchantId = processReconciliationResponse.MerchantId;
            reconciliationResponseMessage.EstateId = processReconciliationResponse.EstateId;
            reconciliationResponseMessage.TransactionId = processReconciliationResponse.TransactionId;

            return reconciliationResponseMessage;
        }

        public GetVoucherResponseMessage ConvertFrom(GetVoucherResponse model)
        {
            if (model == null)
            {
                return null;
            }

            GetVoucherResponseMessage responseMessage = new GetVoucherResponseMessage
            {
                ContractId = model.ContractId,
                EstateId = model.EstateId,
                VoucherCode = model.VoucherCode,
                VoucherId = model.VoucherId,
                ExpiryDate = model.ExpiryDate,
                Value = model.Value,
                ResponseMessage = model.ResponseMessage,
                ResponseCode = model.ResponseCode,
                Balance = model.Balance
            };

            return responseMessage;
        }

        public List<ContractResponse> ConvertFrom(List<Models.ContractResponse> model)
        {
            if (model == null)
            {
                return null;
            }

            List<ContractResponse> responses = new();

            foreach (Models.ContractResponse contractModel in model)
            {
                ContractResponse contractResponse = new()
                {
                    ContractId = contractModel.ContractId,
                    ContractReportingId = contractModel.ContractReportingId,
                    Description = contractModel.Description,
                    EstateId = contractModel.EstateId,
                    EstateReportingId = contractModel.EstateReportingId,
                    OperatorId = contractModel.OperatorId,
                    OperatorName = contractModel.OperatorName,
                    Products = new()
                };

                foreach (ContractProduct contractModelProduct in contractModel.Products)
                {
                    ContractProduct productResponse = new()
                    {
                        Value = contractModelProduct.Value,
                        DisplayText = contractModelProduct.DisplayText,
                        Name = contractModelProduct.Name,
                        ProductId = contractModelProduct.ProductId,
                        ProductReportingId = contractModelProduct.ProductReportingId,
                        ProductType = contractModelProduct.ProductType switch
                        {
                            ProductType.BillPayment => DataTransferObjects.Responses.ProductType.BillPayment,
                            ProductType.MobileTopup => DataTransferObjects.Responses.ProductType.MobileTopup,
                            ProductType.Voucher => DataTransferObjects.Responses.ProductType.Voucher,
                            _ => DataTransferObjects.Responses.ProductType.NotSet
                        },
                        TransactionFees = new()
                    };

                    foreach (ContractProductTransactionFee contractProductTransactionFeeModel in contractModelProduct.TransactionFees)
                    {
                        productResponse.TransactionFees.Add(new DataTransferObjects.Responses.ContractProductTransactionFee
                        {
                            Value = contractProductTransactionFeeModel.Value,
                            Description = contractProductTransactionFeeModel.Description,
                            CalculationType = contractProductTransactionFeeModel.CalculationType switch
                            {
                                CalculationType.Fixed => DataTransferObjects.Responses.CalculationType.Fixed,
                                _ => DataTransferObjects.Responses.CalculationType.Percentage,
                            },
                            FeeType = contractProductTransactionFeeModel.FeeType switch
                            {
                                FeeType.Merchant => DataTransferObjects.Responses.FeeType.Merchant,
                                _ => DataTransferObjects.Responses.FeeType.ServiceProvider,
                            },
                            TransactionFeeId = contractProductTransactionFeeModel.TransactionFeeId,
                            TransactionFeeReportingId = contractProductTransactionFeeModel.TransactionFeeReportingId
                        });
                    }

                    contractResponse.Products.Add(productResponse);
                }

                responses.Add(contractResponse);
            }

            return responses;
        }

        public RedeemVoucherResponseMessage ConvertFrom(RedeemVoucherResponse model)
        {
            if (model == null)
            {
                return null;
            }

            RedeemVoucherResponseMessage responseMessage = new RedeemVoucherResponseMessage
            {
                VoucherCode = model.VoucherCode,
                Balance = model.Balance,
                ResponseMessage = model.ResponseMessage,
                ContractId = model.ContractId,
                EstateId = model.EstateId,
                ExpiryDate = model.ExpiryDate,
                ResponseCode = model.ResponseCode
            };

            return responseMessage;
        }

        #endregion
    }
}
