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
            ModelFactory modelFactory = new ModelFactory();

            ProcessLogonTransactionResponse processLogonTransactionResponse = TestData.ProcessLogonTransactionResponse;

            LogonTransactionResponseMessage response = modelFactory.ConvertFrom(processLogonTransactionResponse);

            response.ShouldNotBeNull();
            response.ResponseMessage.ShouldBe(processLogonTransactionResponse.ResponseMessage);
            response.ResponseCode.ShouldBe(processLogonTransactionResponse.ResponseCode);
            response.EstateId.ShouldBe(processLogonTransactionResponse.EstateId);
            response.MerchantId.ShouldBe(processLogonTransactionResponse.MerchantId);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessLogonTransactionResponse_NullValue_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessLogonTransactionResponse processLogonTransactionResponse = null;

            LogonTransactionResponseMessage response = modelFactory.ConvertFrom(processLogonTransactionResponse);

            response.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessSaleTransactionResponse_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessSaleTransactionResponse processSaleTransactionResponse = TestData.ProcessSaleTransactionResponse;

            SaleTransactionResponseMessage response = modelFactory.ConvertFrom(processSaleTransactionResponse);

            response.ShouldNotBeNull();
            response.ResponseMessage.ShouldBe(processSaleTransactionResponse.ResponseMessage);
            response.ResponseCode.ShouldBe(processSaleTransactionResponse.ResponseCode);
            response.EstateId.ShouldBe(processSaleTransactionResponse.EstateId);
            response.MerchantId.ShouldBe(processSaleTransactionResponse.MerchantId);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessSaleTransactionResponse_NullValue_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessSaleTransactionResponse processSaleTransactionResponse = null;

            SaleTransactionResponseMessage response = modelFactory.ConvertFrom(processSaleTransactionResponse);

            response.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessReconciliationResponse_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessReconciliationResponse processReconciliationResponse = TestData.ProcessReconciliationResponse;

            ReconciliationResponseMessage response = modelFactory.ConvertFrom(processReconciliationResponse);

            response.ShouldNotBeNull();
            response.ResponseMessage.ShouldBe(processReconciliationResponse.ResponseMessage);
            response.ResponseCode.ShouldBe(processReconciliationResponse.ResponseCode);
            response.EstateId.ShouldBe(processReconciliationResponse.EstateId);
            response.MerchantId.ShouldBe(processReconciliationResponse.MerchantId);
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessReconciliationResponse_NullValue_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessReconciliationResponse processReconciliationResponse = null;

            ReconciliationResponseMessage response = modelFactory.ConvertFrom(processReconciliationResponse);

            response.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_GetVoucherResponse_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            GetVoucherResponse model = TestData.GetVoucherResponseModel;
            GetVoucherResponseMessage dto = modelFactory.ConvertFrom(model);

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
            ModelFactory modelFactory = new ModelFactory();

            GetVoucherResponse model = null;
            GetVoucherResponseMessage dto = modelFactory.ConvertFrom(model);

            dto.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_RedeemVoucherResponse_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            RedeemVoucherResponse model = TestData.RedeemVoucherResponseModel;
            RedeemVoucherResponseMessage dto = modelFactory.ConvertFrom(model);

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
            ModelFactory modelFactory = new ModelFactory();

            RedeemVoucherResponse model = null;
            RedeemVoucherResponseMessage dto = modelFactory.ConvertFrom(model);

            dto.ShouldBeNull();
        }

        [Fact]
        public void ModelFactory_ConvertFrom_MerchantResponse_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

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

            DataTransferObjects.Responses.MerchantResponse dto = modelFactory.ConvertFrom(model);

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
            ModelFactory modelFactory = new ModelFactory();

            Models.MerchantResponse model = null;
            DataTransferObjects.Responses.MerchantResponse dto = modelFactory.ConvertFrom(model);

            dto.ShouldBeNull();
        }
    }
}
