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
    }
}
