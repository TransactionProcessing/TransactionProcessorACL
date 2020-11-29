﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.3.0.0
//      SpecFlow Generator Version:3.1.0.0
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.3.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Xunit.TraitAttribute("Category", "base")]
    [Xunit.TraitAttribute("Category", "shared")]
    public partial class SalesTransactionFeature : object, Xunit.IClassFixture<SalesTransactionFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
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
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SalesTransaction", null, ProgrammingLanguage.CSharp, new string[] {
                        "base",
                        "shared"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 4
#line hidden
            TechTalk.SpecFlow.Table table32 = new TechTalk.SpecFlow.Table(new string[] {
                        "RoleName"});
            table32.AddRow(new string[] {
                        "Merchant"});
#line 6
 testRunner.Given("the following security roles exist", ((string)(null)), table32, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table33 = new TechTalk.SpecFlow.Table(new string[] {
                        "ResourceName",
                        "DisplayName",
                        "Secret",
                        "Scopes",
                        "UserClaims"});
            table33.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST",
                        "Secret1",
                        "estateManagement",
                        "MerchantId, EstateId, role"});
            table33.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST",
                        "Secret1",
                        "transactionProcessor",
                        ""});
            table33.AddRow(new string[] {
                        "transactionProcessorACL",
                        "Transaction Processor ACL REST",
                        "Secret1",
                        "transactionProcessorACL",
                        "MerchantId, EstateId, role"});
#line 10
 testRunner.Given("the following api resources exist", ((string)(null)), table33, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table34 = new TechTalk.SpecFlow.Table(new string[] {
                        "ClientId",
                        "ClientName",
                        "Secret",
                        "AllowedScopes",
                        "AllowedGrantTypes"});
            table34.AddRow(new string[] {
                        "serviceClient",
                        "Service Client",
                        "Secret1",
                        "estateManagement,transactionProcessor,transactionProcessorACL",
                        "client_credentials"});
            table34.AddRow(new string[] {
                        "merchantClient",
                        "Merchant Client",
                        "Secret1",
                        "transactionProcessorACL",
                        "password"});
#line 16
 testRunner.Given("the following clients exist", ((string)(null)), table34, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table35 = new TechTalk.SpecFlow.Table(new string[] {
                        "ClientId"});
            table35.AddRow(new string[] {
                        "serviceClient"});
#line 21
 testRunner.Given("I have a token to access the estate management and transaction processor acl reso" +
                    "urces", ((string)(null)), table35, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table36 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName"});
            table36.AddRow(new string[] {
                        "Test Estate 1"});
            table36.AddRow(new string[] {
                        "Test Estate 2"});
#line 25
 testRunner.Given("I have created the following estates", ((string)(null)), table36, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table37 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "RequireCustomMerchantNumber",
                        "RequireCustomTerminalNumber"});
            table37.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "True",
                        "True"});
            table37.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "True",
                        "True"});
#line 30
 testRunner.Given("I have created the following operators", ((string)(null)), table37, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table38 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription"});
            table38.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract"});
            table38.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract"});
#line 35
 testRunner.Given("I create a contract with the following values", ((string)(null)), table38, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table39 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "DisplayText",
                        "Value"});
            table39.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        ""});
            table39.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        ""});
#line 40
 testRunner.When("I create the following Products", ((string)(null)), table39, "When ");
#line hidden
            TechTalk.SpecFlow.Table table40 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "CalculationType",
                        "FeeDescription",
                        "Value"});
            table40.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Fixed",
                        "Merchant Commission",
                        "2.50"});
            table40.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Percentage",
                        "Merchant Commission",
                        "0.85"});
#line 45
 testRunner.When("I add the following Transaction Fees", ((string)(null)), table40, "When ");
#line hidden
            TechTalk.SpecFlow.Table table41 = new TechTalk.SpecFlow.Table(new string[] {
                        "MerchantName",
                        "AddressLine1",
                        "Town",
                        "Region",
                        "Country",
                        "ContactName",
                        "EmailAddress",
                        "EstateName"});
            table41.AddRow(new string[] {
                        "Test Merchant 1",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 1",
                        "testcontact1@merchant1.co.uk",
                        "Test Estate 1"});
            table41.AddRow(new string[] {
                        "Test Merchant 2",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 2",
                        "testcontact2@merchant2.co.uk",
                        "Test Estate 1"});
            table41.AddRow(new string[] {
                        "Test Merchant 3",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 3",
                        "testcontact3@merchant2.co.uk",
                        "Test Estate 2"});
#line 50
 testRunner.Given("I create the following merchants", ((string)(null)), table41, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table42 = new TechTalk.SpecFlow.Table(new string[] {
                        "EmailAddress",
                        "Password",
                        "GivenName",
                        "FamilyName",
                        "EstateName",
                        "MerchantName"});
            table42.AddRow(new string[] {
                        "merchantuser@testmerchant1.co.uk",
                        "123456",
                        "TestMerchant",
                        "User1",
                        "Test Estate 1",
                        "Test Merchant 1"});
            table42.AddRow(new string[] {
                        "merchantuser@testmerchant2.co.uk",
                        "123456",
                        "TestMerchant",
                        "User2",
                        "Test Estate 1",
                        "Test Merchant 2"});
            table42.AddRow(new string[] {
                        "merchantuser@testmerchant3.co.uk",
                        "123456",
                        "TestMerchant",
                        "User3",
                        "Test Estate 2",
                        "Test Merchant 3"});
#line 56
 testRunner.Given("I have created the following security users", ((string)(null)), table42, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table43 = new TechTalk.SpecFlow.Table(new string[] {
                        "OperatorName",
                        "MerchantName",
                        "MerchantNumber",
                        "TerminalNumber",
                        "EstateName"});
            table43.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 1",
                        "00000001",
                        "10000001",
                        "Test Estate 1"});
            table43.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 2",
                        "00000002",
                        "10000002",
                        "Test Estate 1"});
            table43.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 3",
                        "00000003",
                        "10000003",
                        "Test Estate 2"});
#line 62
 testRunner.Given("I have assigned the following  operator to the merchants", ((string)(null)), table43, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table44 = new TechTalk.SpecFlow.Table(new string[] {
                        "DeviceIdentifier",
                        "MerchantName",
                        "EstateName"});
            table44.AddRow(new string[] {
                        "123456780",
                        "Test Merchant 1",
                        "Test Estate 1"});
            table44.AddRow(new string[] {
                        "123456781",
                        "Test Merchant 2",
                        "Test Estate 1"});
            table44.AddRow(new string[] {
                        "123456782",
                        "Test Merchant 3",
                        "Test Estate 2"});
#line 68
 testRunner.Given("I have assigned the following devices to the merchants", ((string)(null)), table44, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table45 = new TechTalk.SpecFlow.Table(new string[] {
                        "Reference",
                        "Amount",
                        "DateTime",
                        "MerchantName",
                        "EstateName"});
            table45.AddRow(new string[] {
                        "Deposit1",
                        "200.00",
                        "Today",
                        "Test Merchant 1",
                        "Test Estate 1"});
            table45.AddRow(new string[] {
                        "Deposit1",
                        "100.00",
                        "Today",
                        "Test Merchant 2",
                        "Test Estate 1"});
            table45.AddRow(new string[] {
                        "Deposit1",
                        "100.00",
                        "Today",
                        "Test Merchant 3",
                        "Test Estate 2"});
#line 74
 testRunner.Given("I make the following manual merchant deposits", ((string)(null)), table45, "Given ");
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
        public virtual void SaleTransaction()
        {
            string[] tagsOfScenario = new string[] {
                    "PRTest"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Sale Transaction", null, tagsOfScenario, argumentsOfScenario);
#line 81
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
#line 82
 testRunner.Given("I am logged in as \"merchantuser@testmerchant1.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 1\" for Estate \"Test Estate 1\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table46 = new TechTalk.SpecFlow.Table(new string[] {
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
                            "ProductName"});
                table46.AddRow(new string[] {
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
                            "Variable Topup"});
                table46.AddRow(new string[] {
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
                            "Variable Topup"});
#line 83
 testRunner.When("I perform the following transactions", ((string)(null)), table46, "When ");
#line hidden
#line 88
 testRunner.Given("I am logged in as \"merchantuser@testmerchant2.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 2\" for Estate \"Test Estate 1\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table47 = new TechTalk.SpecFlow.Table(new string[] {
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
                            "ProductName"});
                table47.AddRow(new string[] {
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
                            "Variable Topup"});
#line 89
 testRunner.When("I perform the following transactions", ((string)(null)), table47, "When ");
#line hidden
#line 93
 testRunner.Given("I am logged in as \"merchantuser@testmerchant3.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 3\" for Estate \"Test Estate 2\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table48 = new TechTalk.SpecFlow.Table(new string[] {
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
                            "ProductName"});
                table48.AddRow(new string[] {
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
                            "Variable Topup"});
#line 94
 testRunner.When("I perform the following transactions", ((string)(null)), table48, "When ");
#line hidden
                TechTalk.SpecFlow.Table table49 = new TechTalk.SpecFlow.Table(new string[] {
                            "EstateName",
                            "MerchantName",
                            "TransactionNumber",
                            "TransactionType",
                            "ResponseCode",
                            "ResponseMessage"});
                table49.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "1",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table49.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 2",
                            "2",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table49.AddRow(new string[] {
                            "Test Estate 2",
                            "Test Merchant 3",
                            "3",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table49.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "4",
                            "Sale",
                            "0000",
                            "SUCCESS"});
#line 98
 testRunner.Then("transaction response should contain the following information", ((string)(null)), table49, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.3.0.0")]
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
