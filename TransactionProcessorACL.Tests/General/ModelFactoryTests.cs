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
        }

        [Fact]
        public void ModelFactory_ConvertFrom_ProcessLogonTransactionResponse_NullValue_IsConverted()
        {
            ModelFactory modelFactory = new ModelFactory();

            ProcessLogonTransactionResponse processLogonTransactionResponse = null;

            LogonTransactionResponseMessage response = modelFactory.ConvertFrom(processLogonTransactionResponse);

            response.ShouldBeNull();
        }
    }
}
