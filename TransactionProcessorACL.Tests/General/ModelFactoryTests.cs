using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.Tests.General
{
    using DataTransferObjects.Responses;
    using Factories;
    using Models;
    using Shouldly;
    using Testing;
    using Xunit;

    public class ModelFactoryTests
    {
        [Fact]
        public void ModelFactory_ConvertFrom_ProcessLogonTransactionResponse_IsConverted()
        {
            ProcessLogonTransactionResponse processLogonTransactionResponse = TestData.ProcessLogonTransactionResponse;

            LogonTransactionResponseMessage response = ModelFactory.ConvertFrom(processLogonTransactionResponse);

            response.ShouldNotBeNull();
            response.ResponseMessage.ShouldBe(processLogonTransactionResponse.ResponseMessage);
            response.ResponseCode.ShouldBe(processLogonTransactionResponse.ResponseCode);
            response.EstateId.ShouldBe(processLogonTransactionResponse.EstateId);
            response.MerchantId.ShouldBe(processLogonTransactionResponse.MerchantId);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessLogonTransactionResponse_NullValue_IsConverted()
        {
            ProcessLogonTransactionResponse processLogonTransactionResponse = null;

            LogonTransactionResponseMessage response = ModelFactory.ConvertFrom(processLogonTransactionResponse);

            response.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessSaleTransactionResponse_IsConverted()
        {
            ProcessSaleTransactionResponse processSaleTransactionResponse = TestData.ProcessSaleTransactionResponse;

            SaleTransactionResponseMessage response = ModelFactory.ConvertFrom(processSaleTransactionResponse);

            response.ShouldNotBeNull();
            response.ResponseMessage.ShouldBe(processSaleTransactionResponse.ResponseMessage);
            response.ResponseCode.ShouldBe(processSaleTransactionResponse.ResponseCode);
            response.EstateId.ShouldBe(processSaleTransactionResponse.EstateId);
            response.MerchantId.ShouldBe(processSaleTransactionResponse.MerchantId);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessSaleTransactionResponse_NullValue_IsConverted()
        {
            ProcessSaleTransactionResponse processSaleTransactionResponse = null;

            SaleTransactionResponseMessage response = ModelFactory.ConvertFrom(processSaleTransactionResponse);

            response.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessReconciliationResponse_IsConverted()
        {
            ProcessReconciliationResponse processReconciliationResponse = TestData.ProcessReconciliationResponse;

            ReconciliationResponseMessage response = ModelFactory.ConvertFrom(processReconciliationResponse);

            response.ShouldNotBeNull();
            response.ResponseMessage.ShouldBe(processReconciliationResponse.ResponseMessage);
            response.ResponseCode.ShouldBe(processReconciliationResponse.ResponseCode);
            response.EstateId.ShouldBe(processReconciliationResponse.EstateId);
            response.MerchantId.ShouldBe(processReconciliationResponse.MerchantId);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessReconciliationResponse_NullValue_IsConverted()
        {
            ProcessReconciliationResponse processReconciliationResponse = null;

            ReconciliationResponseMessage response = ModelFactory.ConvertFrom(processReconciliationResponse);

            response.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_GetVoucherResponse_IsConverted()
        {
            GetVoucherResponse model = TestData.GetVoucherResponseModel;
            GetVoucherResponseMessage dto = ModelFactory.ConvertFrom(model);

            dto.ShouldNotBeNull();
            dto.ContractId.ShouldBe(model.ContractId);
            dto.VoucherCode.ShouldBe(model.VoucherCode);
            dto.EstateId.ShouldBe(model.EstateId);
            dto.Value.ShouldBe(model.Value);
            dto.ExpiryDate.ShouldBe(model.ExpiryDate);
            dto.VoucherId.ShouldBe(model.VoucherId);
            dto.ResponseMessage.ShouldBe(model.ResponseMessage);
            dto.ResponseCode.ShouldBe(model.ResponseCode);
            dto.Balance.ShouldBe(model.Balance);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_GetVoucherResponse_NullValue_IsConverted()
        {
            GetVoucherResponse model = null;
            GetVoucherResponseMessage dto = ModelFactory.ConvertFrom(model);

            dto.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_RedeemVoucherResponse_IsConverted()
        {
            RedeemVoucherResponse model = TestData.RedeemVoucherResponseModel;
            RedeemVoucherResponseMessage dto = ModelFactory.ConvertFrom(model);

            dto.ShouldNotBeNull();
            dto.ContractId.ShouldBe(model.ContractId);
            dto.VoucherCode.ShouldBe(model.VoucherCode);
            dto.EstateId.ShouldBe(model.EstateId);
            dto.Balance.ShouldBe(model.Balance);
            dto.ExpiryDate.ShouldBe(model.ExpiryDate);
            dto.ResponseMessage.ShouldBe(model.ResponseMessage);
            dto.ResponseCode.ShouldBe(model.ResponseCode);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_RedeemVoucherResponse_NullValue_IsConverted()
        {
            RedeemVoucherResponse model = null;
            RedeemVoucherResponseMessage dto = ModelFactory.ConvertFrom(model);

            dto.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_MerchantResponse_IsConverted()
        {
            Models.MerchantResponse model = new Models.MerchantResponse
            {
                EstateId = TestData.EstateId,
                MerchantId = TestData.MerchantId,
                EstateReportingId = 123,
                MerchantReportingId = 456,
                MerchantName = TestData.MerchantName,
                MerchantReference = "Reference",
                NextStatementDate = TestData.GeneratedDateTime,
                SettlementSchedule = Models.SettlementSchedule.Monthly,
                Addresses = new List<Models.AddressResponse>
                {
                    new Models.AddressResponse
                    {
                        AddressId = Guid.NewGuid(),
                        AddressLine1 = TestData.AddressLine1,
                        AddressLine2 = TestData.AddressLine2,
                        AddressLine3 = TestData.AddressLine3,
                        AddressLine4 = TestData.AddressLine4,
                        Country = TestData.Country,
                        PostalCode = TestData.PostCode,
                        Region = TestData.Region,
                        Town = TestData.Town
                    }
                },
                Contacts = new List<Models.ContactResponse>
                {
                    new Models.ContactResponse
                    {
                        ContactId = Guid.NewGuid(),
                        ContactName = TestData.ContactName,
                        ContactPhoneNumber = TestData.ContactPhone,
                        ContactEmailAddress = TestData.ContactEmail
                    }
                },
                Contracts = new List<Models.MerchantContractResponse>
                {
                    new Models.MerchantContractResponse
                    {
                        ContractId = TestData.ContractId,
                        IsDeleted = true,
                        ContractProducts = new List<Guid> { TestData.ProductId }
                    }
                },
                Devices = new Dictionary<Guid, string>
                {
                    { TestData.DeviceId, TestData.DeviceIdentifier }
                },
                Operators = new List<Models.MerchantOperatorResponse>
                {
                    new Models.MerchantOperatorResponse
                    {
                        OperatorId = TestData.OperatorId,
                        IsDeleted = true,
                        MerchantNumber = TestData.MerchantNumber,
                        Name = "Operator Name",
                        TerminalNumber = TestData.TerminalNumber
                    }
                }
            };

            DataTransferObjects.Responses.MerchantResponse dto = ModelFactory.ConvertFrom(model);

            dto.ShouldNotBeNull();
            dto.EstateId.ShouldBe(model.EstateId);
            dto.MerchantId.ShouldBe(model.MerchantId);
            dto.EstateReportingId.ShouldBe(model.EstateReportingId);
            dto.MerchantReportingId.ShouldBe(model.MerchantReportingId);
            dto.MerchantName.ShouldBe(model.MerchantName);
            dto.MerchantReference.ShouldBe(model.MerchantReference);
            dto.NextStatementDate.ShouldBe(model.NextStatementDate);
            dto.SettlementSchedule.ShouldBe(DataTransferObjects.Responses.SettlementSchedule.Monthly);
            dto.Addresses.Count.ShouldBe(1);
            dto.Addresses[0].Town.ShouldBe(TestData.Town);
            dto.Contacts.Count.ShouldBe(1);
            dto.Contacts[0].ContactEmailAddress.ShouldBe(TestData.ContactEmail);
            dto.Contracts.Count.ShouldBe(1);
            dto.Contracts[0].ContractProducts.ShouldContain(TestData.ProductId);
            dto.Devices[TestData.DeviceId].ShouldBe(TestData.DeviceIdentifier);
            dto.Operators.Count.ShouldBe(1);
            dto.Operators[0].TerminalNumber.ShouldBe(TestData.TerminalNumber);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_MerchantResponse_NullValue_IsConverted()
        {
            Models.MerchantResponse model = null;
            DataTransferObjects.Responses.MerchantResponse dto = ModelFactory.ConvertFrom(model);

            dto.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ContractResponses_IsConverted()
        {
            Guid contractId = Guid.NewGuid();
            Guid estateId = Guid.NewGuid();
            Guid operatorId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();
            Guid transactionFeeId = Guid.NewGuid();
            List<Models.ContractResponse> model = new()
            {
                new Models.ContractResponse
                {
                    ContractId = contractId,
                    ContractReportingId = 1,
                    Description = "Contract 1",
                    EstateId = estateId,
                    EstateReportingId = 2,
                    OperatorId = operatorId,
                    OperatorName = "Operator 1",
                    Products = new List<Models.ContractProduct>
                    {
                        new() {
                            DisplayText = "Display",
                            Name = "Product",
                            ProductId = productId,
                            ProductReportingId = 3,
                            Value = 12.34m,
                            ProductType = Models.ProductType.BillPayment,
                            TransactionFees = new List<Models.ContractProductTransactionFee>
                            {
                                new() {
                                    CalculationType = Models.CalculationType.Fixed,
                                    FeeType = Models.FeeType.Merchant,
                                    Description = "Fee",
                                    TransactionFeeId = transactionFeeId,
                                    TransactionFeeReportingId = 4,
                                    Value = 1.23m
                                }
                            }
                        },
                        new() {
                            DisplayText = "Display 2",
                            Name = "Product 2",
                            ProductId = Guid.NewGuid(),
                            ProductReportingId = 5,
                            Value = 23.45m,
                            ProductType = Models.ProductType.MobileTopup,
                            TransactionFees = new List<Models.ContractProductTransactionFee>
                            {
                                new() {
                                    CalculationType = Models.CalculationType.Percentage,
                                    FeeType = Models.FeeType.ServiceProvider,
                                    Description = "Fee 2",
                                    TransactionFeeId = Guid.NewGuid(),
                                    TransactionFeeReportingId = 6,
                                    Value = 2.34m
                                }
                            }
                        },
                        new() {
                            DisplayText = "Display 3",
                            Name = "Product 3",
                            ProductId = Guid.NewGuid(),
                            ProductReportingId = 7,
                            Value = 34.56m,
                            ProductType = Models.ProductType.Voucher,
                            TransactionFees = new List<Models.ContractProductTransactionFee>()
                        }
                    }
                }
            };

            List<DataTransferObjects.Responses.ContractResponse> dto = ModelFactory.ConvertFrom(model);

            dto.ShouldNotBeNull();
            dto.Count.ShouldBe(1);
            dto[0].ContractId.ShouldBe(contractId);
            dto[0].ContractReportingId.ShouldBe(1);
            dto[0].Description.ShouldBe("Contract 1");
            dto[0].EstateId.ShouldBe(estateId);
            dto[0].EstateReportingId.ShouldBe(2);
            dto[0].OperatorId.ShouldBe(operatorId);
            dto[0].OperatorName.ShouldBe("Operator 1");
            dto[0].Products.Count.ShouldBe(3);
            dto[0].Products[0].DisplayText.ShouldBe("Display");
            dto[0].Products[0].Name.ShouldBe("Product");
            dto[0].Products[0].ProductId.ShouldBe(productId);
            dto[0].Products[0].ProductReportingId.ShouldBe(3);
            dto[0].Products[0].Value.ShouldBe(12.34m);
            dto[0].Products[0].ProductType.ShouldBe(DataTransferObjects.Responses.ProductType.BillPayment);
            dto[0].Products[0].TransactionFees.Count.ShouldBe(1);
            dto[0].Products[0].TransactionFees[0].CalculationType.ShouldBe(DataTransferObjects.Responses.CalculationType.Fixed);
            dto[0].Products[0].TransactionFees[0].FeeType.ShouldBe(DataTransferObjects.Responses.FeeType.Merchant);
            dto[0].Products[0].TransactionFees[0].Description.ShouldBe("Fee");
            dto[0].Products[0].TransactionFees[0].TransactionFeeId.ShouldBe(transactionFeeId);
            dto[0].Products[0].TransactionFees[0].TransactionFeeReportingId.ShouldBe(4);
            dto[0].Products[0].TransactionFees[0].Value.ShouldBe(1.23m);
            dto[0].Products[1].ProductType.ShouldBe(DataTransferObjects.Responses.ProductType.MobileTopup);
            dto[0].Products[1].TransactionFees[0].CalculationType.ShouldBe(DataTransferObjects.Responses.CalculationType.Percentage);
            dto[0].Products[1].TransactionFees[0].FeeType.ShouldBe(DataTransferObjects.Responses.FeeType.ServiceProvider);
            dto[0].Products[2].ProductType.ShouldBe(DataTransferObjects.Responses.ProductType.Voucher);
            dto[0].Products[2].TransactionFees.ShouldBeEmpty();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ContractResponses_NullValue_IsConverted()
        {
            List<Models.ContractResponse> model = null;
            List<DataTransferObjects.Responses.ContractResponse> dto = ModelFactory.ConvertFrom(model);

            dto.Count.ShouldBe(0);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ContractResponses_UnknownProductType_IsMappedToNotSet() {
            List<Models.ContractResponse> model = new() { new Models.ContractResponse { Products = new List<Models.ContractProduct> { new Models.ContractProduct { ProductType = (Models.ProductType)999, TransactionFees = new List<Models.ContractProductTransactionFee>() } } } };

            List<DataTransferObjects.Responses.ContractResponse> dto = ModelFactory.ConvertFrom(model);

            dto[0].Products[0].ProductType.ShouldBe(DataTransferObjects.Responses.ProductType.NotSet);
        }
    }
}
