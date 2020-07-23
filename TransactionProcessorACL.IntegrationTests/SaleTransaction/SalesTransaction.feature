﻿@base @shared
Feature: SalesTransaction

Background: 

	Given the following security roles exist
	| RoleName |
	| Merchant   |

	Given the following api resources exist
	| ResourceName            | DisplayName                    | Secret  | Scopes                  | UserClaims                 |
	| estateManagement        | Estate Managememt REST         | Secret1 | estateManagement        | MerchantId, EstateId, role |
	| transactionProcessor    | Transaction Processor REST     | Secret1 | transactionProcessor    |                            |
	| transactionProcessorACL | Transaction Processor ACL REST | Secret1 | transactionProcessorACL | MerchantId, EstateId, role |

	Given the following clients exist
	| ClientId       | ClientName      | Secret  | AllowedScopes                                                 | AllowedGrantTypes  |
	| serviceClient  | Service Client  | Secret1 | estateManagement,transactionProcessor,transactionProcessorACL | client_credentials |
	| merchantClient | Merchant Client | Secret1 | transactionProcessorACL                                       | password           |

	Given I have a token to access the estate management and transaction processor acl resources
	| ClientId      | 
	| serviceClient | 

	Given I have created the following estates
	| EstateName    |
	| Test Estate 1 |
	| Test Estate 2 |

	Given I have created the following operators
	| EstateName    | OperatorName | RequireCustomMerchantNumber | RequireCustomTerminalNumber |
	| Test Estate 1 | Safaricom    | True                        | True                        |
	| Test Estate 2 | Safaricom    | True                        | True                        |

	Given I create a contract with the following values
	| EstateName    | OperatorName    | ContractDescription |
	| Test Estate 1 | Safaricom | Safaricom Contract |
	| Test Estate 2 | Safaricom | Safaricom Contract |

	When I create the following Products
	| EstateName    | OperatorName    | ContractDescription | ProductName    | DisplayText | Value  |
	| Test Estate 1 | Safaricom | Safaricom Contract | Variable Topup | Custom      |        |
	| Test Estate 2 | Safaricom | Safaricom Contract | Variable Topup | Custom      |        |

	When I add the following Transaction Fees
	| EstateName    | OperatorName | ContractDescription | ProductName    | CalculationType | FeeDescription      | Value |
	| Test Estate 1 | Safaricom    | Safaricom Contract  | Variable Topup | Fixed           | Merchant Commission | 2.50  |
	| Test Estate 2 | Safaricom    | Safaricom Contract  | Variable Topup | Percentage      | Merchant Commission | 0.85  |

	Given I create the following merchants
	| MerchantName    | AddressLine1   | Town     | Region      | Country        | ContactName    | EmailAddress                 | EstateName    |
	| Test Merchant 1 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 1 | testcontact1@merchant1.co.uk | Test Estate 1 |
	| Test Merchant 2 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 2 | testcontact2@merchant2.co.uk | Test Estate 1 |
	| Test Merchant 3 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 3 | testcontact3@merchant2.co.uk | Test Estate 2 |

	Given I have created the following security users
	| EmailAddress                  | Password | GivenName    | FamilyName | EstateName    | MerchantName    |
	| merchantuser@testmerchant1.co.uk | 123456   | TestMerchant | User1      | Test Estate 1 | Test Merchant 1 |
	| merchantuser@testmerchant2.co.uk | 123456   | TestMerchant | User2      | Test Estate 1 | Test Merchant 2 |
	| merchantuser@testmerchant3.co.uk | 123456   | TestMerchant | User3      | Test Estate 2 | Test Merchant 3 |

	Given I have assigned the following  operator to the merchants
	| OperatorName | MerchantName    | MerchantNumber | TerminalNumber | EstateName    |
	| Safaricom    | Test Merchant 1 | 00000001       | 10000001       | Test Estate 1 |
	| Safaricom    | Test Merchant 2 | 00000002       | 10000002       | Test Estate 1 |
	| Safaricom    | Test Merchant 3 | 00000003       | 10000003       | Test Estate 2 |

	Given I have assigned the following devices to the merchants
	| DeviceIdentifier | MerchantName    | EstateName    |
	| 123456780        | Test Merchant 1 | Test Estate 1 |
	| 123456781        | Test Merchant 2 | Test Estate 1 |
	| 123456782        | Test Merchant 3 | Test Estate 2 |

	Given I make the following manual merchant deposits 
	| Reference | Amount  | DateTime | MerchantName    | EstateName    |
	| Deposit1  | 2000.00 | Today    | Test Merchant 1 | Test Estate 1 |
	| Deposit1  | 1000.00 | Today    | Test Merchant 2 | Test Estate 1 |
	| Deposit1  | 1000.00 | Today    | Test Merchant 3 | Test Estate 2 |

@PRTest
Scenario: Sale Transaction
	Given I am logged in as "merchantuser@testmerchant1.co.uk" with password "123456" for Merchant "Test Merchant 1" for Estate "Test Estate 1" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | DeviceIdentifier | EstateName    | OperatorName | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress        | ContractDescription | ProductName    |
	| Today    | 1                 | Sale            | Test Merchant 1 | 123456780        | Test Estate 1 | Safaricom    | 1000.00           | 123456789             |                             | Safaricom Contract  | Variable Topup |
	| Today    | 4                 | Sale            | Test Merchant 1 | 123456780        | Test Estate 1 | Safaricom    | 1000.00           | 123456789             | testcustomer@customer.co.uk | Safaricom Contract  | Variable Topup |
	
	Given I am logged in as "merchantuser@testmerchant2.co.uk" with password "123456" for Merchant "Test Merchant 2" for Estate "Test Estate 1" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | DeviceIdentifier | EstateName    | OperatorName | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress | ContractDescription | ProductName    |
	| Today    | 2                 | Sale            | Test Merchant 2 | 123456781        | Test Estate 1 | Safaricom    | 1000.00           | 123456789             |                      | Safaricom Contract  | Variable Topup |
	
	Given I am logged in as "merchantuser@testmerchant3.co.uk" with password "123456" for Merchant "Test Merchant 3" for Estate "Test Estate 2" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | DeviceIdentifier | EstateName    | OperatorName | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress | ContractDescription | ProductName    |
	| Today    | 3                 | Sale            | Test Merchant 3 | 123456782        | Test Estate 2 | Safaricom    | 1000.00           | 123456789             |                      | Safaricom Contract  | Variable Topup |
	
	Then transaction response should contain the following information
	| EstateName    | MerchantName    | TransactionNumber | TransactionType | ResponseCode | ResponseMessage |
	| Test Estate 1 | Test Merchant 1 | 1                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 2 | 2                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 2 | Test Merchant 3 | 3                 | Sale            | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 1 | 4                 | Sale            | 0000         | SUCCESS         |
