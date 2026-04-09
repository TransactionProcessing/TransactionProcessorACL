namespace TransactionProcessorACL.Factories
{
    using System;
    using System.Collections.Generic;
    using DataTransferObjects.Responses;
    using Models;

    public static class ModelFactory
    {
        public static LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse)
        {
            if (processLogonTransactionResponse == null) {
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

        public static SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse)
        {
            if (processSaleTransactionResponse == null) {
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

        public static ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse)
        {
            if (processReconciliationResponse == null) {
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

        public static GetVoucherResponseMessage ConvertFrom(GetVoucherResponse model)
        {
            if (model == null) {
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

        public static DataTransferObjects.Responses.AddressResponse ConvertFrom(Models.AddressResponse model)
        {
            DataTransferObjects.Responses.AddressResponse addressResponse = new() {
                AddressId = model.AddressId,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                AddressLine3 = model.AddressLine3,
                AddressLine4 = model.AddressLine4,
                Country = model.Country,
                PostalCode = model.PostalCode,
                Region = model.Region,
                Town = model.Town
            };
            return addressResponse;
        }

        public static DataTransferObjects.Responses.ContactResponse ConvertFrom(Models.ContactResponse model)
        {
            DataTransferObjects.Responses.ContactResponse contactResponse = new() {
                ContactId = model.ContactId,
                ContactName = model.ContactName,
                ContactPhoneNumber = model.ContactPhoneNumber,
                ContactEmailAddress = model.ContactEmailAddress
            };
            return contactResponse;
        }

        public static DataTransferObjects.Responses.MerchantContractResponse ConvertFrom(Models.MerchantContractResponse model)
        {
            DataTransferObjects.Responses.MerchantContractResponse merchantContractResponse = new() {
                ContractId = model.ContractId,
                IsDeleted = model.IsDeleted,
                ContractProducts = new List<Guid>(model.ContractProducts)
            };
            return merchantContractResponse;
        }

        public static DataTransferObjects.Responses.MerchantOperatorResponse ConvertFrom(Models.MerchantOperatorResponse model)
        {
            DataTransferObjects.Responses.MerchantOperatorResponse merchantOperatorResponse = new() {
                OperatorId = model.OperatorId,
                IsDeleted = model.IsDeleted,
                MerchantNumber = model.MerchantNumber,
                Name = model.Name,
                TerminalNumber = model.TerminalNumber
            };
            return merchantOperatorResponse;
        }

        public static DataTransferObjects.Responses.MerchantResponse ConvertFrom(Models.MerchantResponse model)
        {
            if (model == null) {
                return null;
            }

            DataTransferObjects.Responses.MerchantResponse merchantResponse = new() {
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

            foreach (Models.AddressResponse addressModel in model.Addresses) {
                merchantResponse.Addresses.Add(ConvertFrom(addressModel));
            }

            foreach (Models.ContactResponse contactModel in model.Contacts) {
                merchantResponse.Contacts.Add(ConvertFrom(contactModel));
            }

            foreach (Models.MerchantContractResponse merchantContractResponse in model.Contracts) {
                merchantResponse.Contracts.Add(ConvertFrom(merchantContractResponse));
            }

            foreach (KeyValuePair<Guid, string> device in model.Devices) {
                merchantResponse.Devices.Add(device.Key, device.Value);
            }

            foreach (Models.MerchantOperatorResponse merchantOperatorResponse in model.Operators) {
                merchantResponse.Operators.Add(ConvertFrom(merchantOperatorResponse));
            }

            return merchantResponse;
        }

        public static List<DataTransferObjects.Responses.ContractResponse> ConvertFrom(List<Models.ContractResponse> model) {
            List<DataTransferObjects.Responses.ContractResponse> responses = new();

            if (model == null) {
                return responses;
            }

            foreach (Models.ContractResponse contractModel in model) {
                DataTransferObjects.Responses.ContractResponse contractResponse = ConvertFrom(contractModel);
                
                foreach (Models.ContractProduct contractModelProduct in contractModel.Products) {
                    DataTransferObjects.Responses.ContractProduct contractProductResponse = ConvertFrom(contractModelProduct);
                    foreach (Models.ContractProductTransactionFee contractProductTransactionFeeModel in contractModelProduct.TransactionFees) {
                        contractProductResponse.TransactionFees.Add(ConvertFrom(contractProductTransactionFeeModel));
                    }
                    contractResponse.Products.Add(contractProductResponse);
                }
                responses.Add(contractResponse);
            }

            return responses;
        }

        public static DataTransferObjects.Responses.ContractResponse ConvertFrom(Models.ContractResponse model) {
            DataTransferObjects.Responses.ContractResponse contractResponse = new()
            {
                ContractId = model.ContractId,
                ContractReportingId = model.ContractReportingId,
                Description = model.Description,
                EstateId = model.EstateId,
                EstateReportingId = model.EstateReportingId,
                OperatorId = model.OperatorId,
                OperatorName = model.OperatorName,
                Products = new()
            };

            return contractResponse;
        }

        public static DataTransferObjects.Responses.ContractProduct ConvertFrom(Models.ContractProduct model) {
            DataTransferObjects.Responses.ContractProduct response = new() {
                Value = model.Value,
                DisplayText = model.DisplayText,
                Name = model.Name,
                ProductId = model.ProductId,
                ProductReportingId = model.ProductReportingId,
                ProductType = model.ProductType switch {
                    Models.ProductType.NotSet => DataTransferObjects.Responses.ProductType.NotSet,
                    Models.ProductType.BillPayment => DataTransferObjects.Responses.ProductType.BillPayment,
                    Models.ProductType.MobileTopup => DataTransferObjects.Responses.ProductType.MobileTopup,
                    Models.ProductType.Voucher => DataTransferObjects.Responses.ProductType.Voucher,
                    _ => DataTransferObjects.Responses.ProductType.NotSet
                },
                TransactionFees = new()
            };
            return response;
        }

        public static DataTransferObjects.Responses.ContractProductTransactionFee ConvertFrom(Models.ContractProductTransactionFee model) {
            DataTransferObjects.Responses.ContractProductTransactionFee response = new DataTransferObjects.Responses.ContractProductTransactionFee
            {
                Value = model.Value,
                Description = model.Description,
                CalculationType = model.CalculationType switch
                {
                    Models.CalculationType.Fixed => DataTransferObjects.Responses.CalculationType.Fixed,
                    _ => DataTransferObjects.Responses.CalculationType.Percentage,
                },
                FeeType = model.FeeType switch
                {
                    Models.FeeType.Merchant => DataTransferObjects.Responses.FeeType.Merchant,
                    _ => DataTransferObjects.Responses.FeeType.ServiceProvider,
                },
                TransactionFeeId = model.TransactionFeeId,
                TransactionFeeReportingId = model.TransactionFeeReportingId
            };
            return response;
        }

        public static RedeemVoucherResponseMessage ConvertFrom(RedeemVoucherResponse model)
        {
            if (model == null) {
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
    }
}
