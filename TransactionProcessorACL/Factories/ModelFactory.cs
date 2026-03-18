namespace TransactionProcessorACL.Factories
{
    using System;
    using System.Collections.Generic;
    using DataTransferObjects.Responses;
    using Models;

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

        public DataTransferObjects.Responses.MerchantResponse ConvertFrom(Models.MerchantResponse model)
        {
            if (model == null)
            {
                return null;
            }

            DataTransferObjects.Responses.MerchantResponse merchantResponse = new DataTransferObjects.Responses.MerchantResponse
            {
                EstateId = model.EstateId,
                MerchantId = model.MerchantId,
                EstateReportingId = model.EstateReportingId,
                MerchantName = model.MerchantName,
                MerchantReference = model.MerchantReference,
                MerchantReportingId = model.MerchantReportingId,
                NextStatementDate = model.NextStatementDate,
                SettlementSchedule = model.SettlementSchedule switch
                {
                    Models.SettlementSchedule.Weekly => DataTransferObjects.Responses.SettlementSchedule.Weekly,
                    Models.SettlementSchedule.Monthly => DataTransferObjects.Responses.SettlementSchedule.Monthly,
                    _ => DataTransferObjects.Responses.SettlementSchedule.NotSet
                },
                Addresses = new List<DataTransferObjects.Responses.AddressResponse>(),
                Contacts = new List<DataTransferObjects.Responses.ContactResponse>(),
                Contracts = new List<DataTransferObjects.Responses.MerchantContractResponse>(),
                Devices = new Dictionary<Guid, string>(),
                Operators = new List<DataTransferObjects.Responses.MerchantOperatorResponse>()
            };

            foreach (Models.AddressResponse addressModel in model.Addresses)
            {
                merchantResponse.Addresses.Add(new DataTransferObjects.Responses.AddressResponse
                {
                    AddressId = addressModel.AddressId,
                    AddressLine1 = addressModel.AddressLine1,
                    AddressLine2 = addressModel.AddressLine2,
                    AddressLine3 = addressModel.AddressLine3,
                    AddressLine4 = addressModel.AddressLine4,
                    Country = addressModel.Country,
                    PostalCode = addressModel.PostalCode,
                    Region = addressModel.Region,
                    Town = addressModel.Town
                });
            }

            foreach (Models.ContactResponse contactModel in model.Contacts)
            {
                merchantResponse.Contacts.Add(new DataTransferObjects.Responses.ContactResponse
                {
                    ContactId = contactModel.ContactId,
                    ContactName = contactModel.ContactName,
                    ContactPhoneNumber = contactModel.ContactPhoneNumber,
                    ContactEmailAddress = contactModel.ContactEmailAddress
                });
            }

            foreach (Models.MerchantContractResponse merchantContractResponse in model.Contracts)
            {
                DataTransferObjects.Responses.MerchantContractResponse contract = new DataTransferObjects.Responses.MerchantContractResponse
                {
                    ContractId = merchantContractResponse.ContractId,
                    IsDeleted = merchantContractResponse.IsDeleted,
                    ContractProducts = new List<Guid>(merchantContractResponse.ContractProducts)
                };

                merchantResponse.Contracts.Add(contract);
            }

            foreach (KeyValuePair<Guid, string> device in model.Devices)
            {
                merchantResponse.Devices.Add(device.Key, device.Value);
            }

            foreach (Models.MerchantOperatorResponse merchantOperatorResponse in model.Operators)
            {
                merchantResponse.Operators.Add(new DataTransferObjects.Responses.MerchantOperatorResponse
                {
                    OperatorId = merchantOperatorResponse.OperatorId,
                    IsDeleted = merchantOperatorResponse.IsDeleted,
                    MerchantNumber = merchantOperatorResponse.MerchantNumber,
                    Name = merchantOperatorResponse.Name,
                    TerminalNumber = merchantOperatorResponse.TerminalNumber
                });
            }

            return merchantResponse;
        }

        public List<DataTransferObjects.Responses.ContractResponse> ConvertFrom(List<Models.ContractResponse> model)
        {
            if (model == null)
            {
                return null;
            }

            List<DataTransferObjects.Responses.ContractResponse> responses = new();

            foreach (Models.ContractResponse contractModel in model)
            {
                DataTransferObjects.Responses.ContractResponse contractResponse = new()
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

                foreach (Models.ContractProduct contractModelProduct in contractModel.Products)
                {
                    DataTransferObjects.Responses.ContractProduct contractProductResponse = new()
                    {
                        Value = contractModelProduct.Value,
                        DisplayText = contractModelProduct.DisplayText,
                        Name = contractModelProduct.Name,
                        ProductId = contractModelProduct.ProductId,
                        ProductReportingId = contractModelProduct.ProductReportingId,
                        ProductType = contractModelProduct.ProductType switch
                        {
                            Models.ProductType.NotSet => DataTransferObjects.Responses.ProductType.NotSet,
                            Models.ProductType.BillPayment => DataTransferObjects.Responses.ProductType.BillPayment,
                            Models.ProductType.MobileTopup => DataTransferObjects.Responses.ProductType.MobileTopup,
                            Models.ProductType.Voucher => DataTransferObjects.Responses.ProductType.Voucher,
                            _ => DataTransferObjects.Responses.ProductType.NotSet
                        },
                        TransactionFees = new()
                    };

                    foreach (Models.ContractProductTransactionFee contractProductTransactionFeeModel in contractModelProduct.TransactionFees)
                    {
                        contractProductResponse.TransactionFees.Add(new DataTransferObjects.Responses.ContractProductTransactionFee
                        {
                            Value = contractProductTransactionFeeModel.Value,
                            Description = contractProductTransactionFeeModel.Description,
                            CalculationType = contractProductTransactionFeeModel.CalculationType switch
                            {
                                Models.CalculationType.Fixed => DataTransferObjects.Responses.CalculationType.Fixed,
                                _ => DataTransferObjects.Responses.CalculationType.Percentage,
                            },
                            FeeType = contractProductTransactionFeeModel.FeeType switch
                            {
                                Models.FeeType.Merchant => DataTransferObjects.Responses.FeeType.Merchant,
                                _ => DataTransferObjects.Responses.FeeType.ServiceProvider,
                            },
                            TransactionFeeId = contractProductTransactionFeeModel.TransactionFeeId,
                            TransactionFeeReportingId = contractProductTransactionFeeModel.TransactionFeeReportingId
                        });
                    }

                    contractResponse.Products.Add(contractProductResponse);
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
