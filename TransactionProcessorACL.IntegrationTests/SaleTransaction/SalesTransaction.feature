@base @shared
Feature: SalesTransaction

Background: 

	Given the following security roles exist
	| Role Name |
	| Merchant   |

	Given I create the following api scopes
	| Name                 | DisplayName                       | Description                            |
	| transactionProcessor | Transaction Processor REST  Scope | A scope for Transaction Processor REST |
	| transactionProcessorACL | Transaction Processor ACL REST  Scope | A scope for Transaction Processor ACL REST |
	
	Given the following api resources exist
	| Name            | DisplayName                    | Secret  | Scopes                  | UserClaims                 |
	| transactionProcessor    | Transaction Processor REST     | Secret1 | transactionProcessor    | merchantId, estateId, role |
	| transactionProcessorACL | Transaction Processor ACL REST | Secret1 | transactionProcessorACL | merchantId, estateId, role |
	
	Given the following clients exist
	| ClientId       | ClientName      | Secret  | Scopes                                                                   | GrantTypes  |
	| serviceClient  | Service Client  | Secret1 | transactionProcessor,transactionProcessorACL | client_credentials |
	| merchantClient | Merchant Client | Secret1 | transactionProcessorACL                                                         | password           |

	Given I have a token to access the estate management and transaction processor acl resources
	| ClientId      | 
	| serviceClient | 

	Given the following bills are available at the PataPawa PostPaid Host
	| AccountNumber | AccountName    | DueDate | Amount |
	| 12345678      | Test Account 1 | Today   | 100.00 |

	Given the following users are available at the PataPawa PrePay Host
	| Username | Password |
	| operatora    | 1234567898   |

	Given the following meters are available at the PataPawa PrePay Host
	| MeterNumber | CustomerName |
	| 00000001    | Customer 1   |
	| 00000002    | Customer 2   |
	| 00000003    | Customer 3   |

	Given I initialise the Agency Banking Host

	Given I create the following accounts
	| Type      | Code   | Name                   | Currency |
	| LIABILITY | 200100 | Agent Float GL         | KES      |
	| LIABILITY | 200200 | Settlement Suspense GL | KES      |
	| EXPENSE   | 400100 | Agent Commission GL    | KES      |
	| LIABILITY | 200300 | Reversal GL            | KES      |
	| INCOME    | 400200 | Fee Income GL          | KES      |

	Given I create the settlement account
	| AccountNumber | BankCode | Currency | AccountName                    |
	|     999000001 |      001 | KES      | Main Settlement Account |

	Given I create the following customers
	| CustomerId | FullName        | PhoneNumber | NationalId | AccountNumber |
	| CUST001    | Test Customer 1 |  0712345678 |   12345678 |      12345678 |

	Given I have created the following estates
	| EstateName    |
	| Test Estate 1 |
	| Test Estate 2 |

	Given I have created the following operators
	| EstateName     | OperatorName     | RequireCustomMerchantNumber | RequireCustomTerminalNumber |
	| Test Estate 1  | Safaricom        | True                        | True                        |
	| Test Estate 1  | Voucher          | True                        | True                        |
	| Test Estate 1  | PataPawa PostPay | False                       | False                       |
	| Test Estate 1  | PataPawa PrePay  | False                       | False                       |
	| Test Estate 1  | AgencyBanking    | False                       | False                       |
	| Test Estate 2  | Safaricom        | True                        | True                        |
	| Test Estate 2  | Voucher          | True                        | True                        |
	| Test Estate 2  | PataPawa PostPay | False                       | False                       |
	| Test Estate 2  | PataPawa PrePay  | False                       | False                       |
	| Test Estate 2 | AgencyBanking    | False                       | False                       |

	And I have assigned the following operators to the estates
	| EstateName    | OperatorName    | 
	| Test Estate 1 | Safaricom |
	| Test Estate 1 | Voucher |
	| Test Estate 1 | PataPawa PostPay |
	| Test Estate 1 | PataPawa PrePay |
	| Test Estate 1 | AgencyBanking |
	| Test Estate 2 | Safaricom |
	| Test Estate 2 | Voucher |
	| Test Estate 2 | PataPawa PostPay |
	| Test Estate 2 | PataPawa PrePay |
	| Test Estate 2 | AgencyBanking |

	Given I create a contract with the following values
	| EstateName    | OperatorName    | ContractDescription |
	| Test Estate 1 | Safaricom | Safaricom Contract |
	| Test Estate 1 | Voucher      | Hospital 1 Contract |
	| Test Estate 1 | PataPawa PostPay | PataPawa PostPay Contract |
	| Test Estate 1 | PataPawa PrePay | PataPawa PrePay Contract |
	| Test Estate 1 | AgencyBanking    | AgencyBanking Contract    |
	| Test Estate 2 | Safaricom | Safaricom Contract |
	| Test Estate 2 | Voucher      | Hospital 1 Contract |
	| Test Estate 2 | PataPawa PostPay | PataPawa PostPay Contract |
	| Test Estate 2 | PataPawa PrePay | PataPawa PrePay Contract |
	| Test Estate 2 | AgencyBanking    | AgencyBanking Contract    |

	When I create the following Products
	| EstateName    | OperatorName     | ContractDescription       | ProductName       | DisplayText     | Value | ProductType |
	| Test Estate 1 | Safaricom        | Safaricom Contract        | Variable Topup    | Custom          |       | MobileTopup |
	| Test Estate 1 | Voucher          | Hospital 1 Contract       | 10 KES            | 10 KES          |       | Voucher     |
	| Test Estate 1 | PataPawa PostPay | PataPawa PostPay Contract | Post Pay Bill Pay | Bill Pay (Post) |       | BillPayment |
	| Test Estate 1 | PataPawa PrePay  | PataPawa PrePay Contract  | Pre Pay Bill Pay  | Bill Pay (Pre)  |       | BillPayment |
	| Test Estate 1 | AgencyBanking    | AgencyBanking Contract    | Balance Enquiry   | Balance Enquiry |       | MobileTopup |
	| Test Estate 1 | AgencyBanking    | AgencyBanking Contract    | Deposit           | Deposit         |       | MobileTopup |
	| Test Estate 1 | AgencyBanking    | AgencyBanking Contract    | Withdrawal        | Withdrawal      |       | MobileTopup |
	| Test Estate 1 | AgencyBanking    | AgencyBanking Contract    | MiniStatement     | Mini Statement  |       | MobileTopup |
	| Test Estate 2 | Safaricom        | Safaricom Contract        | Variable Topup    | Custom          |       | MobileTopup |
	| Test Estate 2 | Voucher          | Hospital 1 Contract       | 10 KES            | 10 KES          |       | Voucher     |
	| Test Estate 2 | PataPawa PostPay | PataPawa PostPay Contract | Post Pay Bill Pay | Bill Pay (Post) |       | BillPayment |
	| Test Estate 2 | PataPawa PrePay  | PataPawa PrePay Contract  | Pre Pay Bill Pay  | Bill Pay (Pre)  |       | BillPayment |
	| Test Estate 2 | AgencyBanking    | AgencyBanking Contract    | Balance Enquiry   | Balance Enquiry |       | MobileTopup |
	| Test Estate 2 | AgencyBanking    | AgencyBanking Contract    | Deposit           | Deposit         |       | MobileTopup |
	| Test Estate 2 | AgencyBanking    | AgencyBanking Contract    | Withdrawal        | Withdrawal      |       | MobileTopup |
	| Test Estate 2 | AgencyBanking    | AgencyBanking Contract    | MiniStatement     | Mini Statement  |       | MobileTopup |

	When I add the following Transaction Fees
	| EstateName    | OperatorName     | ContractDescription       | ProductName       | CalculationType | FeeDescription      | Value |
	| Test Estate 1 | Safaricom        | Safaricom Contract        | Variable Topup    | Fixed           | Merchant Commission |  2.50 |
	| Test Estate 1 | PataPawa PostPay | PataPawa PostPay Contract | Post Pay Bill Pay | Percentage      | Merchant Commission |  0.50 |
	| Test Estate 1 | PataPawa PrePay  | PataPawa PrePay Contract  | Pre Pay Bill Pay  | Percentage      | Merchant Commission |  0.50 |
	| Test Estate 2 | Safaricom        | Safaricom Contract        | Variable Topup    | Percentage      | Merchant Commission |  0.85 |
	| Test Estate 2 | PataPawa PostPay | PataPawa PostPay Contract | Post Pay Bill Pay | Percentage      | Merchant Commission |  0.50 |
	| Test Estate 2 | PataPawa PrePay  | PataPawa PrePay Contract  | Pre Pay Bill Pay  | Percentage      | Merchant Commission |  0.50 |

	Given I create the following merchants
	| MerchantName    | AddressLine1   | Town     | Region      | PostalCode |Country        | ContactName    | EmailAddress                 | EstateName    | EnableAgencyBanking |
	| Test Merchant 1 | Address Line 1 | TestTown | Test Region | TE57 1NG   |United Kingdom | Test Contact 1 | testcontact1@merchant1.co.uk | Test Estate 1 | True                |
	| Test Merchant 2 | Address Line 1 | TestTown | Test Region | TE57 2NG   |United Kingdom | Test Contact 2 | testcontact2@merchant2.co.uk | Test Estate 1 | False               |
	| Test Merchant 3 | Address Line 1 | TestTown | Test Region | TE57 3NG   |United Kingdom | Test Contact 3 | testcontact3@merchant2.co.uk | Test Estate 2 | False               |

	Given I have assigned the following  operator to the merchants
	| OperatorName     | MerchantName    | MerchantNumber | TerminalNumber | EstateName    |
	| Safaricom        | Test Merchant 1 |       00000001 |       10000001 | Test Estate 1 |
	| Voucher          | Test Merchant 1 |       00000001 |       10000001 | Test Estate 1 |
	| PataPawa PostPay | Test Merchant 1 |       00000001 |       10000001 | Test Estate 1 |
	| PataPawa PrePay  | Test Merchant 1 |       00000001 |       10000001 | Test Estate 1 |
	| AgencyBanking    | Test Merchant 1 |       00000001 |       10000001 | Test Estate 1 |
	| Safaricom        | Test Merchant 2 |       00000002 |       10000002 | Test Estate 1 |
	| Voucher          | Test Merchant 2 |       00000002 |       10000002 | Test Estate 1 |
	| PataPawa PostPay | Test Merchant 2 |       00000001 |       10000001 | Test Estate 1 |
	| PataPawa PrePay  | Test Merchant 2 |       00000001 |       10000001 | Test Estate 1 |
	| AgencyBanking    | Test Merchant 2 |       00000001 |       10000001 | Test Estate 1 |
	| Safaricom        | Test Merchant 3 |       00000003 |       10000003 | Test Estate 2 |
	| Voucher          | Test Merchant 3 |       00000003 |       10000003 | Test Estate 2 |
	| PataPawa PostPay | Test Merchant 3 |       00000001 |       10000001 | Test Estate 2 |
	| PataPawa PrePay  | Test Merchant 3 |       00000001 |       10000001 | Test Estate 2 |
	| AgencyBanking    | Test Merchant 3 |       00000001 |       10000001 | Test Estate 2 |

	Given I have assigned the following devices to the merchants
	| DeviceIdentifier | MerchantName    | EstateName    |
	| 123456780        | Test Merchant 1 | Test Estate 1 |
	| 123456781        | Test Merchant 2 | Test Estate 1 |
	| 123456782        | Test Merchant 3 | Test Estate 2 |

	When I add the following contracts to the following merchants
	| EstateName    | MerchantName    | ContractDescription       |
	| Test Estate 1 | Test Merchant 1 | Safaricom Contract        |
	| Test Estate 1 | Test Merchant 1 | Hospital 1 Contract       |
	| Test Estate 1 | Test Merchant 1 | PataPawa PostPay Contract |
	| Test Estate 1 | Test Merchant 1 | PataPawa PrePay Contract |
	| Test Estate 1 | Test Merchant 1 | AgencyBanking Contract    |
	| Test Estate 1 | Test Merchant 2 | Safaricom Contract        |
	| Test Estate 1 | Test Merchant 2 | Hospital 1 Contract       |
	| Test Estate 1 | Test Merchant 2 | PataPawa PostPay Contract |
	| Test Estate 1 | Test Merchant 2 | PataPawa PrePay Contract |
	| Test Estate 1 | Test Merchant 2 | AgencyBanking Contract    |
	| Test Estate 2 | Test Merchant 3 | Safaricom Contract        |
	| Test Estate 2 | Test Merchant 3 | Hospital 1 Contract       |
	| Test Estate 2 | Test Merchant 3 | PataPawa PostPay Contract |
	| Test Estate 2 | Test Merchant 3 | PataPawa PrePay Contract |
	| Test Estate 2 | Test Merchant 3 | AgencyBanking Contract    |

	Given I make the following manual merchant deposits 
	| Reference | Amount  | DateTime | MerchantName    | EstateName    |
	| Deposit1  | 365.00 | Today    | Test Merchant 1 | Test Estate 1 |
	| Deposit1  | 110.00 | Today    | Test Merchant 2 | Test Estate 1 |
	| Deposit1  | 110.00 | Today    | Test Merchant 3 | Test Estate 2 |

	Given I have created the following security users
	| EmailAddress                  | Password | GivenName    | FamilyName | EstateName    | MerchantName    |
	| merchantuser@testmerchant1.co.uk | 123456   | TestMerchant | User1      | Test Estate 1 | Test Merchant 1 |
	| merchantuser@testmerchant2.co.uk | 123456   | TestMerchant | User2      | Test Estate 1 | Test Merchant 2 |
	| merchantuser@testmerchant3.co.uk | 123456   | TestMerchant | User3      | Test Estate 2 | Test Merchant 3 |

@PRTest
Scenario: Sale Transaction
	Given I am logged in as "merchantuser@testmerchant1.co.uk" with password "123456" for Merchant "Test Merchant 1" for Estate "Test Estate 1" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | DeviceIdentifier | EstateName    | OperatorName     | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress        | ContractDescription       | ProductName       | RecipientEmail       | RecipientMobile | MessageType    | AccountNumber | CustomerName     | MeterNumber |
	| Today    |                 1 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | Safaricom        |            100.00 |             123456789 |                             | Safaricom Contract        | Variable Topup    |                      |                 |                |               |                  |             |
	| Today    |                 4 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | Safaricom        |            100.00 |             123456789 | testcustomer@customer.co.uk | Safaricom Contract        | Variable Topup    |                      |                 |                |               |                  |             |
	| Today    |                 5 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | Voucher          |             10.00 |                       |                             | Hospital 1 Contract       | 10 KES            | test@recipient.co.uk |                 |                |               |                  |             |
	| Today    |                 8 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | PataPawa PostPay |              0.00 |                       |                             | PataPawa PostPay Contract | Post Pay Bill Pay | test@recipient.co.uk |                 | VerifyAccount  |      12345678 |                  |             |
	| Today    |                 9 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | PataPawa PostPay |             20.00 |                       |                             | PataPawa PostPay Contract | Post Pay Bill Pay | test@recipient.co.uk |       123456789 | ProcessBill    |      12345678 | Mr Test Customer |             |
	| Today    |                10 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | PataPawa PrePay  |              0.00 |                       |                             | PataPawa PrePay Contract  | Pre Pay Bill Pay  | test@recipient.co.uk |                 | meter          |               |                  |    00000001 |
	| Today    |                11 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | PataPawa PrePay  |             25.00 |                       |                             | PataPawa PrePay Contract  | Pre Pay Bill Pay  | test@recipient.co.uk |                 | vend           |      00000001 | Customer 1       |    00000001 |
	| Today    |                12 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | AgencyBanking    |              0.00 |                       |                             | AgencyBanking Contract    | Balance Enquiry   |                      |        12345678 | balanceenquiry |      12345678 |                  |             |
	| Today    |                13 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | AgencyBanking    |            100.00 |                       |                             | AgencyBanking Contract    | Deposit           |                      |        12345678 | deposit        |      12345678 |                  |             |
	| Today    |                14 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | AgencyBanking    |              9.00 |                       |                             | AgencyBanking Contract    | Withdrawal        |                      |        12345678 | withdrawal     |      12345678 |                  |             |
	| Today    |                15 | Sale            | Test Merchant 1 |        123456780 | Test Estate 1 | AgencyBanking    |              0.00 |                       |                             | AgencyBanking Contract    | MiniStatement     |                      |        12345678 | ministatement  |      12345678 |                  |             |
	
	
	Given I am logged in as "merchantuser@testmerchant2.co.uk" with password "123456" for Merchant "Test Merchant 2" for Estate "Test Estate 1" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | DeviceIdentifier | EstateName    | OperatorName | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress | ContractDescription | ProductName    | RecipientEmail | RecipientMobile |
	| Today    | 2                 | Sale            | Test Merchant 2 | 123456781        | Test Estate 1 | Safaricom    | 100.00            | 123456789             |                      | Safaricom Contract  | Variable Topup |                |                 |
	| Today    | 6                 | Sale            | Test Merchant 2 | 123456781        | Test Estate 1 | Voucher      | 10.00             |                       |                      | Hospital 1 Contract | 10 KES         |                | 123456789       |
	
	Given I am logged in as "merchantuser@testmerchant3.co.uk" with password "123456" for Merchant "Test Merchant 3" for Estate "Test Estate 2" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | DeviceIdentifier | EstateName    | OperatorName | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress | ContractDescription | ProductName    | RecipientEmail       | RecipientMobile |
	| Today    | 3                 | Sale            | Test Merchant 3 | 123456782        | Test Estate 2 | Safaricom    | 100.00            | 123456789             |                      | Safaricom Contract  | Variable Topup |                      |                 |
	| Today    | 7                 | Sale            | Test Merchant 3 | 123456782        | Test Estate 2 | Voucher      | 10.00             |                       |                      | Hospital 1 Contract | 10 KES         | test@recipient.co.uk |                 |
	
	Then the sale transaction response should contain the following information
	| EstateName    | MerchantName    | TransactionNumber | TransactionType | ResponseCode | ResponseMessage |
	| Test Estate 1 | Test Merchant 1 | 1                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 2 | 2                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 2 | Test Merchant 3 | 3                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 1 | 4                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 1 | 5                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 2 | 6                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 2 | Test Merchant 3 | 7                 | Sale            | 0000         | SUCCESS         |
