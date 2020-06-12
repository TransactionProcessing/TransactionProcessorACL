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
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessSaleTransactionResponse_NullValue_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessSaleTransactionResponse processSaleTransactionResponse = null;

            SaleTransactionResponseMessage response = modelFactory.ConvertFrom(processSaleTransactionResponse);

            response.ShouldBeNull();
        }
    }
}
