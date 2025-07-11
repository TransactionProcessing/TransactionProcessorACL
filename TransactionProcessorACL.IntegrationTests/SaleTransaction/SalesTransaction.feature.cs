﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
using Reqnroll;
namespace TransactionProcessorACL.IntegrationTests.SaleTransaction
{
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("SalesTransaction")]
    [NUnit.Framework.FixtureLifeCycleAttribute(NUnit.Framework.LifeCycle.InstancePerTestCase)]
    [NUnit.Framework.CategoryAttribute("base")]
    [NUnit.Framework.CategoryAttribute("shared")]
    public partial class SalesTransactionFeature
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "base",
                "shared"};
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new global::System.Globalization.CultureInfo("en-US"), "SaleTransaction", "SalesTransaction", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
#line 1 "SalesTransaction.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public static async global::System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public static async global::System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public async global::System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            try
            {
                if (((testRunner.FeatureContext != null) 
                            && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
                {
                    await testRunner.OnFeatureEndAsync();
                }
            }
            finally
            {
                if (((testRunner.FeatureContext != null) 
                            && testRunner.FeatureContext.BeforeFeatureHookFailed))
                {
                    throw new global::Reqnroll.ReqnrollException("Scenario skipped because of previous before feature hook error");
                }
                if ((testRunner.FeatureContext == null))
                {
                    await testRunner.OnFeatureStartAsync(featureInfo);
                }
            }
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public async global::System.Threading.Tasks.Task TestTearDownAsync()
        {
            if ((testRunner == null))
            {
                return;
            }
            try
            {
                await testRunner.OnScenarioEndAsync();
            }
            finally
            {
                global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
                testRunner = null;
            }
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public async global::System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async global::System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        public virtual async global::System.Threading.Tasks.Task FeatureBackgroundAsync()
        {
#line 4
#line hidden
            global::Reqnroll.Table table73 = new global::Reqnroll.Table(new string[] {
                        "Role Name"});
            table73.AddRow(new string[] {
                        "Merchant"});
#line 6
 await testRunner.GivenAsync("the following security roles exist", ((string)(null)), table73, "Given ");
#line hidden
            global::Reqnroll.Table table74 = new global::Reqnroll.Table(new string[] {
                        "Name",
                        "DisplayName",
                        "Description"});
            table74.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST Scope",
                        "A scope for Estate Managememt REST"});
            table74.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST  Scope",
                        "A scope for Transaction Processor REST"});
            table74.AddRow(new string[] {
                        "transactionProcessorACL",
                        "Transaction Processor ACL REST  Scope",
                        "A scope for Transaction Processor ACL REST"});
            table74.AddRow(new string[] {
                        "voucherManagement",
                        "Voucher Management REST  Scope",
                        "A scope for Voucher Management REST"});
#line 10
 await testRunner.GivenAsync("I create the following api scopes", ((string)(null)), table74, "Given ");
#line hidden
            global::Reqnroll.Table table75 = new global::Reqnroll.Table(new string[] {
                        "Name",
                        "DisplayName",
                        "Secret",
                        "Scopes",
                        "UserClaims"});
            table75.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST",
                        "Secret1",
                        "estateManagement",
                        "merchantId, estateId, role"});
            table75.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST",
                        "Secret1",
                        "transactionProcessor",
                        ""});
            table75.AddRow(new string[] {
                        "transactionProcessorACL",
                        "Transaction Processor ACL REST",
                        "Secret1",
                        "transactionProcessorACL",
                        "merchantId, estateId, role"});
            table75.AddRow(new string[] {
                        "voucherManagement",
                        "Voucher Management REST",
                        "Secret1",
                        "voucherManagement",
                        ""});
#line 17
 await testRunner.GivenAsync("the following api resources exist", ((string)(null)), table75, "Given ");
#line hidden
            global::Reqnroll.Table table76 = new global::Reqnroll.Table(new string[] {
                        "ClientId",
                        "ClientName",
                        "Secret",
                        "Scopes",
                        "GrantTypes"});
            table76.AddRow(new string[] {
                        "serviceClient",
                        "Service Client",
                        "Secret1",
                        "estateManagement,transactionProcessor,transactionProcessorACL,voucherManagement",
                        "client_credentials"});
            table76.AddRow(new string[] {
                        "merchantClient",
                        "Merchant Client",
                        "Secret1",
                        "transactionProcessorACL",
                        "password"});
#line 24
 await testRunner.GivenAsync("the following clients exist", ((string)(null)), table76, "Given ");
#line hidden
            global::Reqnroll.Table table77 = new global::Reqnroll.Table(new string[] {
                        "ClientId"});
            table77.AddRow(new string[] {
                        "serviceClient"});
#line 29
 await testRunner.GivenAsync("I have a token to access the estate management and transaction processor acl reso" +
                    "urces", ((string)(null)), table77, "Given ");
#line hidden
            global::Reqnroll.Table table78 = new global::Reqnroll.Table(new string[] {
                        "EstateName"});
            table78.AddRow(new string[] {
                        "Test Estate 1"});
            table78.AddRow(new string[] {
                        "Test Estate 2"});
#line 33
 await testRunner.GivenAsync("I have created the following estates", ((string)(null)), table78, "Given ");
#line hidden
            global::Reqnroll.Table table79 = new global::Reqnroll.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "RequireCustomMerchantNumber",
                        "RequireCustomTerminalNumber"});
            table79.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "True",
                        "True"});
            table79.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher",
                        "True",
                        "True"});
            table79.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "True",
                        "True"});
            table79.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher",
                        "True",
                        "True"});
#line 38
 await testRunner.GivenAsync("I have created the following operators", ((string)(null)), table79, "Given ");
#line hidden
            global::Reqnroll.Table table80 = new global::Reqnroll.Table(new string[] {
                        "EstateName",
                        "OperatorName"});
            table80.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom"});
            table80.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher"});
            table80.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom"});
            table80.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher"});
#line 45
 await testRunner.AndAsync("I have assigned the following operators to the estates", ((string)(null)), table80, "And ");
#line hidden
            global::Reqnroll.Table table81 = new global::Reqnroll.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription"});
            table81.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract"});
            table81.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher",
                        "Hospital 1 Contract"});
            table81.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract"});
            table81.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher",
                        "Hospital 1 Contract"});
#line 52
 await testRunner.GivenAsync("I create a contract with the following values", ((string)(null)), table81, "Given ");
#line hidden
            global::Reqnroll.Table table82 = new global::Reqnroll.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "DisplayText",
                        "Value",
                        "ProductType"});
            table82.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        "",
                        "MobileTopup"});
            table82.AddRow(new string[] {
                        "Test Estate 1",
                        "Voucher",
                        "Hospital 1 Contract",
                        "10 KES",
                        "10 KES",
                        "",
                        "Voucher"});
            table82.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        "",
                        "MobileTopup"});
            table82.AddRow(new string[] {
                        "Test Estate 2",
                        "Voucher",
                        "Hospital 1 Contract",
                        "10 KES",
                        "10 KES",
                        "",
                        "Voucher"});
#line 59
 await testRunner.WhenAsync("I create the following Products", ((string)(null)), table82, "When ");
#line hidden
            global::Reqnroll.Table table83 = new global::Reqnroll.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "CalculationType",
                        "FeeDescription",
                        "Value"});
            table83.AddRow(new string[] {
                        "Test Estate 1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Fixed",
                        "Merchant Commission",
                        "2.50"});
            table83.AddRow(new string[] {
                        "Test Estate 2",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Percentage",
                        "Merchant Commission",
                        "0.85"});
#line 66
 await testRunner.WhenAsync("I add the following Transaction Fees", ((string)(null)), table83, "When ");
#line hidden
            global::Reqnroll.Table table84 = new global::Reqnroll.Table(new string[] {
                        "MerchantName",
                        "AddressLine1",
                        "Town",
                        "Region",
                        "Country",
                        "ContactName",
                        "EmailAddress",
                        "EstateName"});
            table84.AddRow(new string[] {
                        "Test Merchant 1",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 1",
                        "testcontact1@merchant1.co.uk",
                        "Test Estate 1"});
            table84.AddRow(new string[] {
                        "Test Merchant 2",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 2",
                        "testcontact2@merchant2.co.uk",
                        "Test Estate 1"});
            table84.AddRow(new string[] {
                        "Test Merchant 3",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 3",
                        "testcontact3@merchant2.co.uk",
                        "Test Estate 2"});
#line 71
 await testRunner.GivenAsync("I create the following merchants", ((string)(null)), table84, "Given ");
#line hidden
            global::Reqnroll.Table table85 = new global::Reqnroll.Table(new string[] {
                        "OperatorName",
                        "MerchantName",
                        "MerchantNumber",
                        "TerminalNumber",
                        "EstateName"});
            table85.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 1",
                        "00000001",
                        "10000001",
                        "Test Estate 1"});
            table85.AddRow(new string[] {
                        "Voucher",
                        "Test Merchant 1",
                        "00000001",
                        "10000001",
                        "Test Estate 1"});
            table85.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 2",
                        "00000002",
                        "10000002",
                        "Test Estate 1"});
            table85.AddRow(new string[] {
                        "Voucher",
                        "Test Merchant 2",
                        "00000002",
                        "10000002",
                        "Test Estate 1"});
            table85.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 3",
                        "00000003",
                        "10000003",
                        "Test Estate 2"});
            table85.AddRow(new string[] {
                        "Voucher",
                        "Test Merchant 3",
                        "00000003",
                        "10000003",
                        "Test Estate 2"});
#line 77
 await testRunner.GivenAsync("I have assigned the following  operator to the merchants", ((string)(null)), table85, "Given ");
#line hidden
            global::Reqnroll.Table table86 = new global::Reqnroll.Table(new string[] {
                        "DeviceIdentifier",
                        "MerchantName",
                        "EstateName"});
            table86.AddRow(new string[] {
                        "123456780",
                        "Test Merchant 1",
                        "Test Estate 1"});
            table86.AddRow(new string[] {
                        "123456781",
                        "Test Merchant 2",
                        "Test Estate 1"});
            table86.AddRow(new string[] {
                        "123456782",
                        "Test Merchant 3",
                        "Test Estate 2"});
#line 86
 await testRunner.GivenAsync("I have assigned the following devices to the merchants", ((string)(null)), table86, "Given ");
#line hidden
            global::Reqnroll.Table table87 = new global::Reqnroll.Table(new string[] {
                        "EstateName",
                        "MerchantName",
                        "ContractDescription"});
            table87.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 1",
                        "Safaricom Contract"});
            table87.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 1",
                        "Hospital 1 Contract"});
            table87.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 2",
                        "Safaricom Contract"});
            table87.AddRow(new string[] {
                        "Test Estate 1",
                        "Test Merchant 2",
                        "Hospital 1 Contract"});
            table87.AddRow(new string[] {
                        "Test Estate 2",
                        "Test Merchant 3",
                        "Safaricom Contract"});
            table87.AddRow(new string[] {
                        "Test Estate 2",
                        "Test Merchant 3",
                        "Hospital 1 Contract"});
#line 92
 await testRunner.WhenAsync("I add the following contracts to the following merchants", ((string)(null)), table87, "When ");
#line hidden
            global::Reqnroll.Table table88 = new global::Reqnroll.Table(new string[] {
                        "Reference",
                        "Amount",
                        "DateTime",
                        "MerchantName",
                        "EstateName"});
            table88.AddRow(new string[] {
                        "Deposit1",
                        "210.00",
                        "Today",
                        "Test Merchant 1",
                        "Test Estate 1"});
            table88.AddRow(new string[] {
                        "Deposit1",
                        "110.00",
                        "Today",
                        "Test Merchant 2",
                        "Test Estate 1"});
            table88.AddRow(new string[] {
                        "Deposit1",
                        "110.00",
                        "Today",
                        "Test Merchant 3",
                        "Test Estate 2"});
#line 101
 await testRunner.GivenAsync("I make the following manual merchant deposits", ((string)(null)), table88, "Given ");
#line hidden
            global::Reqnroll.Table table89 = new global::Reqnroll.Table(new string[] {
                        "EmailAddress",
                        "Password",
                        "GivenName",
                        "FamilyName",
                        "EstateName",
                        "MerchantName"});
            table89.AddRow(new string[] {
                        "merchantuser@testmerchant1.co.uk",
                        "123456",
                        "TestMerchant",
                        "User1",
                        "Test Estate 1",
                        "Test Merchant 1"});
            table89.AddRow(new string[] {
                        "merchantuser@testmerchant2.co.uk",
                        "123456",
                        "TestMerchant",
                        "User2",
                        "Test Estate 1",
                        "Test Merchant 2"});
            table89.AddRow(new string[] {
                        "merchantuser@testmerchant3.co.uk",
                        "123456",
                        "TestMerchant",
                        "User3",
                        "Test Estate 2",
                        "Test Merchant 3"});
#line 107
 await testRunner.GivenAsync("I have created the following security users", ((string)(null)), table89, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Sale Transaction")]
        [NUnit.Framework.CategoryAttribute("PRTest")]
        public async global::System.Threading.Tasks.Task SaleTransaction()
        {
            string[] tagsOfScenario = new string[] {
                    "PRTest"};
            global::System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new global::System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Sale Transaction", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 114
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 4
await this.FeatureBackgroundAsync();
#line hidden
#line 115
 await testRunner.GivenAsync("I am logged in as \"merchantuser@testmerchant1.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 1\" for Estate \"Test Estate 1\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
                global::Reqnroll.Table table90 = new global::Reqnroll.Table(new string[] {
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
                table90.AddRow(new string[] {
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
                table90.AddRow(new string[] {
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
                table90.AddRow(new string[] {
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
#line 116
 await testRunner.WhenAsync("I perform the following transactions", ((string)(null)), table90, "When ");
#line hidden
#line 122
 await testRunner.GivenAsync("I am logged in as \"merchantuser@testmerchant2.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 2\" for Estate \"Test Estate 1\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
                global::Reqnroll.Table table91 = new global::Reqnroll.Table(new string[] {
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
                table91.AddRow(new string[] {
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
                table91.AddRow(new string[] {
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
#line 123
 await testRunner.WhenAsync("I perform the following transactions", ((string)(null)), table91, "When ");
#line hidden
#line 128
 await testRunner.GivenAsync("I am logged in as \"merchantuser@testmerchant3.co.uk\" with password \"123456\" for M" +
                        "erchant \"Test Merchant 3\" for Estate \"Test Estate 2\" with client \"merchantClient" +
                        "\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
                global::Reqnroll.Table table92 = new global::Reqnroll.Table(new string[] {
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
                table92.AddRow(new string[] {
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
                table92.AddRow(new string[] {
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
#line 129
 await testRunner.WhenAsync("I perform the following transactions", ((string)(null)), table92, "When ");
#line hidden
                global::Reqnroll.Table table93 = new global::Reqnroll.Table(new string[] {
                            "EstateName",
                            "MerchantName",
                            "TransactionNumber",
                            "TransactionType",
                            "ResponseCode",
                            "ResponseMessage"});
                table93.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "1",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table93.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 2",
                            "2",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table93.AddRow(new string[] {
                            "Test Estate 2",
                            "Test Merchant 3",
                            "3",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table93.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "4",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table93.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 1",
                            "5",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table93.AddRow(new string[] {
                            "Test Estate 1",
                            "Test Merchant 2",
                            "6",
                            "Sale",
                            "0000",
                            "SUCCESS"});
                table93.AddRow(new string[] {
                            "Test Estate 2",
                            "Test Merchant 3",
                            "7",
                            "Sale",
                            "0000",
                            "SUCCESS"});
#line 134
 await testRunner.ThenAsync("the sale transaction response should contain the following information", ((string)(null)), table93, "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
    }
}
#pragma warning restore
#endregion
