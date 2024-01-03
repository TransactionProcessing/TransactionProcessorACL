﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace TransactionProcessorACL.IntegrationTests.SaleTransaction
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Xunit.TraitAttribute("Category", "base")]
    [Xunit.TraitAttribute("Category", "shared")]
    public partial class SalesTransactionFeature : object, Xunit.IClassFixture<SalesTransactionFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "base",
                "shared"};
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SalesTransaction.feature"
#line hidden
        
        public SalesTransactionFeature(SalesTransactionFeature.FixtureData fixtureData, TransactionProcessorACL_IntegrationTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SaleTransaction", "SalesTransaction", null, ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 4
#line hidden
            TechTalk.SpecFlow.Table table51 = new TechTalk.SpecFlow.Table(new string[] {
                        "Role Name"});
            table51.AddRow(new string[] {
                        "Merchant"});
#line 6
 testRunner.Given("the following security roles exist", ((string)(null)), table51, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table52 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "DisplayName",
                        "Description"});
            table52.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST Scope",
                        "A scope for Estate Managememt REST"});
            table52.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST  Scope",
                        "A scope for Transaction Processor REST"});
            table52.AddRow(new string[] {
                        "transactionProcessorACL",
                        "Transaction Processor ACL REST  Scope",
                        "A scope for Transaction Processor ACL REST"});
            table52.AddRow(new string[] {
                        "voucherManagement",
                        "Voucher Management REST  Scope",
                        "A scope for Voucher Management REST"});
#line 10
 testRunner.Given("I create the following api scopes", ((string)(null)), table52, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table53 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "DisplayName",
                        "Secret",
                        "Scopes",
                        "UserClaims"});
            table53.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST",
                        "Secret1",
                        "estateManagement",
                        "merchantId, estateId, role"});
            table53.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST",
                        "Secret1",
                        "transactionProcessor",
                        ""});
            table53.AddRow(new string[] {
                        "transactionProcessorACL",
                        "Transaction Processor ACL REST",
                        "Secret1",
                        "transactionProcessorACL",
                        "merchantId, estateId, role"});
            table53.AddRow(new string[] {
                        "voucherManagement",
                        "Voucher Management REST",
                        "Secret1",
                        "voucherManagement",
                        ""});
#line 17
 testRunner.Given("the following api resources exist", ((string)(null)), table53, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table54 = new TechTalk.SpecFlow.Table(new string[] {
                        "ClientId",
                        "ClientName",
                        "Secret",
                        "Scopes",
                        "GrantTypes"});
            table54.AddRow(new string[] {
                        "serviceClient",
                        "Service Client",
                        "Secret1",
                        "estateManagement,transactionProcessor,transactionProcessorACL,voucherManagement",
                        "client_credentials"});
            table54.AddRow(new string[] {
                        "merchantClient",
                        "Merchant Client",
                        "Secret1",
                        "transactionProcessorACL",
                        "password"});
#line 24
 testRunner.Given("the following clients exist", ((string)(null)), table54, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table55 = new TechTalk.SpecFlow.Table(new string[] {
                        "ClientId"});
            table55.AddRow(new string[] {
                        "serviceClient"});
#line 29
 testRunner.Given("I have a token to access the estate management and transaction processor acl reso" +
                    "urces", ((string)(null)), table55, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table56 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName"});
            table56.AddRow(new string[] {
                        "Test Estate 1"});
            table56.AddRow(new string[] {
                        "Test Estate 2"});
#line 33
 testRunner.Given("I have created the following estates", ((string)(null)), table56, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table57 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "RequireCustomMerchantNumber",
                        "RequireCustomTerminalNumber"});
            table57.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "True",
                        "True"});
            table57.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher",
                        "True",
                        "True"});
            table57.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "True",
                        "True"});
            table57.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher",
                        "True",
                        "True"});
#line 38
 testRunner.Given("I have created the following operators", ((string)(null)), table57, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table58 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription"});
            table58.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract"});
            table58.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher",
                        "Hospital 1 Contract"});
            table58.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract"});
            table58.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher",
                        "Hospital 1 Contract"});
#line 45
 testRunner.Given("I create a contract with the following values", ((string)(null)), table58, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table59 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "DisplayText",
                        "Value",
                        "ProductType"});
            table59.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        "",
                        "MobileTopup"});
            table59.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher",
                        "Hospital 1 Contract",
                        "10 KES",
                        "10 KES",
                        "",
                        "Voucher"});
            table59.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        "",
                        "MobileTopup"});
            table59.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher",
                        "Hospital 1 Contract",
                        "10 KES",
                        "10 KES",
                        "",
                        "Voucher"});
#line 52
 testRunner.When("I create the following Products", ((string)(null)), table59, "When ");
#line hidden
            TechTalk.SpecFlow.Table table60 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "CalculationType",
                        "FeeDescription",
                        "Value"});
            table60.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Fixed",
                        "Merchant Commission",
                        "2.50"});
            table60.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Percentage",
                        "Merchant Commission",
                        "0.85"});
#line 59
 testRunner.When("I add the following Transaction Fees", ((string)(null)), table60, "When ");
#line hidden
            TechTalk.SpecFlow.Table table61 = new TechTalk.SpecFlow.Table(new string[] {
                        "MerchantName",
                        "AddressLine1",
                        "Town",
                        "Region",
                        "Country",
                        "ContactName",
                        "EmailAddress",
                        "EstateName"});
            table61.AddRow(new string[] {
                        "Test Merchant 1",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 1",
                        "testcontact1@merchant1.co.uk",
                        "Test Estate 1"});
            table61.AddRow(new string[] {
                        "Test Merchant 2",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 2",
                        "testcontact2@merchant2.co.uk",
                        "Test Estate 1"});
            table61.AddRow(new string[] {
                        "Test Merchant 3",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 3",
                        "testcontact3@merchant2.co.uk",
                        "Test Estate 2"});
#line 64
 testRunner.Given("I create the following merchants", ((string)(null)), table61, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table62 = new TechTalk.SpecFlow.Table(new string[] {
                        "OperatorName",
                        "MerchantName",
                        "MerchantNumber",
                        "TerminalNumber",
                        "EstateName"});
            table62.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 1",
                        "00000001",
                        "10000001",
                        "Test Estate 1"});
            table62.AddRow(new string[] {
                        "Voucher",
                        "Test Merchant 1",
                        "00000001",
                        "10000001",
                        "Test Estate 1"});
            table62.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 2",
                        "00000002",
                        "10000002",
                        "Test Estate 1"});
            table62.AddRow(new string[] {
                        "Voucher",
                        "Test Merchant 2",
                        "00000002",
                        "10000002",
                        "Test Estate 1"});
            table62.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 3",
                        "00000003",
                        "10000003",
                        "Test Estate 2"});
            table62.AddRow(new string[] {
                        "Voucher",
                        "Test Merchant 3",
                        "00000003",
                        "10000003",
                        "Test Estate 2"});
#line 70
 testRunner.Given("I have assigned the following  operator to the merchants", ((string)(null)), table62, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table63 = new TechTalk.SpecFlow.Table(new string[] {
                        "DeviceIdentifier",
                        "MerchantName",
                        "EstateName"});
            table63.AddRow(new string[] {
                        "123456780",
                        "Test Merchant 1",
                        "Test Estate 1"});
            table63.AddRow(new string[] {
                        "123456781",
                        "Test Merchant 2",
                        "Test Estate 1"});
            table63.AddRow(new string[] {
                        "123456782",
                        "Test Merchant 3",
                        "Test Estate 2"});
#line 79
 testRunner.Given("I have assigned the following devices to the merchants", ((string)(null)), table63, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table64 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "MerchantName",
                        "ContractDescription"});
            table64.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 1",
                        "Safaricom Contract"});
            table64.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 1",
                        "Hospital 1 Contract"});
            table64.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 2",
                        "Safaricom Contract"});
            table64.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 2",
                        "Hospital 1 Contract"});
            table64.AddRow(new string[] {
                        "Test Estate 2",
                        "Test Merchant 3",
                        "Safaricom Contract"});
            table64.AddRow(new string[] {
                        "Test Estate 2",
                        "Test Merchant 3",
                        "Hospital 1 Contract"});
#line 85
 testRunner.When("I add the following contracts to the following merchants", ((string)(null)), table64, "When ");
#line hidden
            TechTalk.SpecFlow.Table table65 = new TechTalk.SpecFlow.Table(new string[] {
                        "Reference",
                        "Amount",
                        "DateTime",
                        "MerchantName",
                        "EstateName"});
            table65.AddRow(new string[] {
                        "Deposit1",
                        "210.00",
                        "Today",
                        "Test Merchant 1",
                        "Test Estate 1"});
            table65.AddRow(new string[] {
                        "Deposit1",
                        "110.00",
                        "Today",
                        "Test Merchant 2",
                        "Test Estate 1"});
            table65.AddRow(new string[] {
                        "Deposit1",
                        "110.00",
                        "Today",
                        "Test Merchant 3",
                        "Test Estate 2"});
#line 94
 testRunner.Given("I make the following manual merchant deposits", ((string)(null)), table65, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table66 = new TechTalk.SpecFlow.Table(new string[] {
                        "EmailAddress",
                        "Password",
                        "GivenName",
                        "FamilyName",
                        "EstateName",
                        "MerchantName"});
            table66.AddRow(new string[] {
                        "merchantuser@testmerchant1.co.uk",
                        "123456",
                        "TestMerchant",
                        "User1",
                        "Test Estate 1",
                        "Test Merchant 1"});
            table66.AddRow(new string[] {
                        "merchantuser@testmerchant2.co.uk",
                        "123456",
                        "TestMerchant",
                        "User2",
                        "Test Estate 1",
                        "Test Merchant 2"});
            table66.AddRow(new string[] {
                        "merchantuser@testmerchant3.co.uk",
                        "123456",
                        "TestMerchant",
                        "User3",
                        "Test Estate 2",
                        "Test Merchant 3"});
#line 100
 testRunner.Given("I have created the following security users", ((string)(null)), table66, "Given ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Sale Transaction")]
        [Xunit.TraitAttribute("FeatureTitle", "SalesTransaction")]
        [Xunit.TraitAttribute("Description", "Sale Transaction")]
        [Xunit.TraitAttribute("Category", "PRTest")]
        public void SaleTransaction()
        {
            string[] tagsOfScenario = new string[] {
                    "PRTest"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Sale Transaction", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 107
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
#line 108
 testRunner.Given("I am logged in as \"merchantuser@testmerchant1.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 1\" for Estate \"Test Estate 1\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table67 = new TechTalk.SpecFlow.Table(new string[] {
                            "DateTime",
                            "TransactionNumber",
                            "TransactionType",
                            "MerchantName",
                            "DeviceIdentifier",
                            "EstateName",
                            "OperatorName",
                            "TransactionAmount",
                            "CustomerAccountNumber",
                            "CustomerEmailAddress",
                            "ContractDescription",
                            "ProductName",
                            "RecipientEmail",
                            "RecipientMobile"});
                table67.AddRow(new string[] {
                            "Today",
                            "1",
                            "Sale",
                            "Test Merchant 1",
                            "123456780",
                            "Test Estate 1",
                            "Safaricom",
                            "100.00",
                            "123456789",
                            "",
                            "Safaricom Contract",
                            "Variable Topup",
                            "",
                            ""});
                table67.AddRow(new string[] {
                            "Today",
                            "4",
                            "Sale",
                            "Test Merchant 1",
                            "123456780",
                            "Test Estate 1",
                            "Safaricom",
                            "100.00",
                            "123456789",
                            "testcustomer@customer.co.uk",
                            "Safaricom Contract",
                            "Variable Topup",
                            "",
                            ""});
                table67.AddRow(new string[] {
                            "Today",
                            "5",
                            "Sale",
                            "Test Merchant 1",
                            "123456780",
                            "Test Estate 1",
                            "Voucher",
                            "10.00",
                            "",
                            "",
                            "Hospital 1 Contract",
                            "10 KES",
                            "test@recipient.co.uk",
                            ""});
#line 109
 testRunner.When("I perform the following transactions", ((string)(null)), table67, "When ");
#line hidden
#line 115
 testRunner.Given("I am logged in as \"merchantuser@testmerchant2.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 2\" for Estate \"Test Estate 1\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table68 = new TechTalk.SpecFlow.Table(new string[] {
                            "DateTime",
                            "TransactionNumber",
                            "TransactionType",
                            "MerchantName",
                            "DeviceIdentifier",
                            "EstateName",
                            "OperatorName",
                            "TransactionAmount",
                            "CustomerAccountNumber",
                            "CustomerEmailAddress",
                            "ContractDescription",
                            "ProductName",
                            "RecipientEmail",
                            "RecipientMobile"});
                table68.AddRow(new string[] {
                            "Today",
                            "2",
                            "Sale",
                            "Test Merchant 2",
                            "123456781",
                            "Test Estate 1",
                            "Safaricom",
                            "100.00",
                            "123456789",
                            "",
                            "Safaricom Contract",
                            "Variable Topup",
                            "",
                            ""});
                table68.AddRow(new string[] {
                            "Today",
                            "6",
                            "Sale",
                            "Test Merchant 2",
                            "123456781",
                            "Test Estate 1",
                            "Voucher",
                            "10.00",
                            "",
                            "",
                            "Hospital 1 Contract",
                            "10 KES",
                            "",
                            "123456789"});
#line 116
 testRunner.When("I perform the following transactions", ((string)(null)), table68, "When ");
#line hidden
#line 121
 testRunner.Given("I am logged in as \"merchantuser@testmerchant3.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 3\" for Estate \"Test Estate 2\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table69 = new TechTalk.SpecFlow.Table(new string[] {
                            "DateTime",
                            "TransactionNumber",
                            "TransactionType",
                            "MerchantName",
                            "DeviceIdentifier",
                            "EstateName",
                            "OperatorName",
                            "TransactionAmount",
                            "CustomerAccountNumber",
                            "CustomerEmailAddress",
                            "ContractDescription",
                            "ProductName",
                            "RecipientEmail",
                            "RecipientMobile"});
                table69.AddRow(new string[] {
                            "Today",
                            "3",
                            "Sale",
                            "Test Merchant 3",
                            "123456782",
                            "Test Estate 2",
                            "Safaricom",
                            "100.00",
                            "123456789",
                            "",
                            "Safaricom Contract",
                            "Variable Topup",
                            "",
                            ""});
                table69.AddRow(new string[] {
                            "Today",
                            "7",
                            "Sale",
                            "Test Merchant 3",
                            "123456782",
                            "Test Estate 2",
                            "Voucher",
                            "10.00",
                            "",
                            "",
                            "Hospital 1 Contract",
                            "10 KES",
                            "test@recipient.co.uk",
                            ""});
#line 122
 testRunner.When("I perform the following transactions", ((string)(null)), table69, "When ");
#line hidden
                TechTalk.SpecFlow.Table table70 = new TechTalk.SpecFlow.Table(new string[] {
                            "EstateName",
                            "MerchantName",
                            "TransactionNumber",
                            "TransactionType",
                            "ResponseCode",
                            "ResponseMessage"});
                table70.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "1",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table70.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 2",
                            "2",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table70.AddRow(new string[] {
                            "Test Estate 2",
                            "Test Merchant 3",
                            "3",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table70.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "4",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table70.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "5",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table70.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 2",
                            "6",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table70.AddRow(new string[] {
                            "Test Estate 2",
                            "Test Merchant 3",
                            "7",
                            "Sale",
                            "0000",
                            "SUCCESS"});
#line 127
 testRunner.Then("the sale transaction response should contain the following information", ((string)(null)), table70, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                SalesTransactionFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SalesTransactionFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
