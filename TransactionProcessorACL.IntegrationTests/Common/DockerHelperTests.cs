using System;
using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using TransactionProcessing.Shared.IntegrationTesting;
using TransactionProcessing.Shared.IntegrationTesting.TestContainers;

namespace TransactionProcessor.IntegrationTests.Common
{
    [TestFixture]
    public class DockerHelperTests
    {
        [Test]
        public void GetAdditionalVariablesForAcl_leaves_common_dependency_urls_to_base_helper()
        {
            TestableDockerHelper dockerHelper = new();
            dockerHelper.RequiredDockerServices = DockerServices.CallbackHandler | DockerServices.KurrentDb |
                                                  DockerServices.FileProcessor | DockerServices.MessagingService |
                                                  DockerServices.SecurityService | DockerServices.SqlServer |
                                                  DockerServices.TestHost | DockerServices.TransactionProcessor |
                                                  DockerServices.TransactionProcessorAcl | DockerServices.EstateReporting;

            Dictionary<string, string> variables = dockerHelper.GetAdditionalVariables(ContainerType.TransactionProcessorAcl);
            Dictionary<string, string> commonVariables = dockerHelper.GetCommonEnvironmentVariables();

            String securityServiceAddress = $"https://{dockerHelper.SecurityServiceName}:5001/";
            commonVariables["SecurityConfiguration:Authority"].ShouldBe(securityServiceAddress.TrimEnd('/'));
            commonVariables["AppSettings:TransactionProcessorApi"].ShouldBe($"http://{dockerHelper.TransactionProcessorName}:5002");
            variables.ShouldNotContainKey("SecurityConfiguration:Authority");
            variables["SecurityConfiguration:Issuer"].ShouldBe(securityServiceAddress);
            variables["SecurityConfiguration:ConfigurationEndpoint"].ShouldBe($"{securityServiceAddress}.well-known/openid-configuration");
            variables.ShouldNotContainKey("AppSettings:TransactionProcessorApi");
        }

        private sealed class TestableDockerHelper : DockerHelper
        {
            public string SecurityServiceName => this.SecurityServiceContainerName;

            public string TransactionProcessorName => this.TransactionProcessorContainerName;
        }
    }
}
