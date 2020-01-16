@base @shared
Feature: LogonTransaction

Background: 

	Given the following security roles exist
	| RoleName |
	| Merchant   |

	Given the following api resources exist
	| ResourceName            | DisplayName                    | Secret  | Scopes                  | UserClaims                 |
	| estateManagement        | Estate Managememt REST         | Secret1 | estateManagement        | MerchantId, EstateId, role |
	| transactionProcessor    | Transaction Processor REST     | Secret1 | transactionProcessor    |                            |
	| transactionProcessorAcl | Transaction Processor ACL REST | Secret1 | transactionProcessorAcl | MerchantId, EstateId, role |

	Given the following clients exist
	| ClientId       | ClientName      | Secret  | AllowedScopes                                                 | AllowedGrantTypes  |
	| serviceClient  | Service Client  | Secret1 | estateManagement,transactionProcessor,transactionProcessorAcl | client_credentials |
	| merchantClient | Merchant Client | Secret1 | transactionProcessorAcl                                       | password           |

	Given I have a token to access the estate management and transaction processor acl resources
	| ClientId      | 
	| serviceClient | 

	Given I have created the following estates
	| EstateName    |
	| Test Estate 1 |
	| Test Estate 2 |

	Given I have created the following operators
	| EstateName    | OperatorName    | RequireCustomMerchantNumber | RequireCustomTerminalNumber |
	| Test Estate 1 | Test Operator 1 | True                        | True                        |
	| Test Estate 2 | Test Operator 1 | True                        | True                        |

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
	| OperatorName    | MerchantName    | MerchantNumber | TerminalNumber | EstateName    |
	| Test Operator 1 | Test Merchant 1 | 00000001       | 10000001       | Test Estate 1 |
	| Test Operator 1 | Test Merchant 2 | 00000001       | 10000001       | Test Estate 1 |
	| Test Operator 1 | Test Merchant 3 | 00000001       | 10000001       | Test Estate 2 |

@PRTest
Scenario: Logon Transaction
	Given I am logged in as "merchantuser@testmerchant1.co.uk" with password "123456" for Merchant "Test Merchant 1" for Estate "Test Estate 1" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | IMEINumber | EstateName    |
	| Today    | 1                 | Logon           | Test Merchant 1 | 123456789  | Test Estate 1 |
	
	Given I am logged in as "merchantuser@testmerchant2.co.uk" with password "123456" for Merchant "Test Merchant 2" for Estate "Test Estate 1" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | IMEINumber | EstateName    |
	| Today    | 2                 | Logon           | Test Merchant 2 | 123456789  | Test Estate 1 |
	
	Given I am logged in as "merchantuser@testmerchant3.co.uk" with password "123456" for Merchant "Test Merchant 3" for Estate "Test Estate 2" with client "merchantClient"
	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | MerchantName    | IMEINumber | EstateName    |
	| Today    | 3                 | Logon           | Test Merchant 3 | 123456789  | Test Estate 2 |
	
	Then transaction response should contain the following information
	| EstateName    | MerchantName    | TransactionNumber | TransactionType | ResponseCode | ResponseMessage |
	| Test Estate 1 | Test Merchant 1 | 1                 | Logon           | 0000         | SUCCESS         |
	| Test Estate 1 | Test Merchant 2 | 2                 | Logon           | 0000         | SUCCESS         |
	| Test Estate 2 | Test Merchant 3 | 3                 | Logon           | 0000         | SUCCESS         |
