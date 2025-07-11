﻿@base @shared
Feature: Merchants

Background: 

	Given the following security roles exist
	| Role Name |
	| Merchant   |

	Given I create the following api scopes
	| Name                 | DisplayName                       | Description                            |
	| estateManagement     | Estate Managememt REST Scope      | A scope for Estate Managememt REST     |
	| transactionProcessor | Transaction Processor REST  Scope | A scope for Transaction Processor REST |
	| transactionProcessorACL | Transaction Processor ACL REST  Scope | A scope for Transaction Processor ACL REST |

	Given the following api resources exist
	| Name            | DisplayName                    | Secret  | Scopes                  | UserClaims                 |
	| estateManagement        | Estate Managememt REST         | Secret1 | estateManagement        | merchantId, estateId, role |
	| transactionProcessor    | Transaction Processor REST     | Secret1 | transactionProcessor    |                            |
	| transactionProcessorACL | Transaction Processor ACL REST | Secret1 | transactionProcessorACL | merchantId, estateId, role |

	Given the following clients exist
	| ClientId       | ClientName      | Secret  | Scopes                                                 | GrantTypes  |
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
	| Test Estate 1 | Voucher      | True                        | True                        |
	| Test Estate 2 | Safaricom    | True                        | True                        |
	| Test Estate 2 | Voucher      | True                        | True                        |

	And I have assigned the following operators to the estates
	| EstateName    | OperatorName |
	| Test Estate 1 | Safaricom    |
	| Test Estate 1 | Voucher      |
	| Test Estate 2 | Safaricom    |
	| Test Estate 2 | Voucher      |

	Given I create the following merchants
	| MerchantName    | AddressLine1   | Town     | Region      | Country        | ContactName    | EmailAddress                 | EstateName    |
	| Test Merchant 1 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 1 | testcontact1@merchant1.co.uk | Test Estate 1 |
	| Test Merchant 2 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 2 | testcontact2@merchant2.co.uk | Test Estate 1 |
	| Test Merchant 3 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 3 | testcontact3@merchant2.co.uk | Test Estate 2 |

	Given I have assigned the following  operator to the merchants
	| OperatorName | MerchantName    | MerchantNumber | TerminalNumber | EstateName    |
	| Safaricom    | Test Merchant 1 | 00000001       | 10000001       | Test Estate 1 |
	| Voucher      | Test Merchant 1 | 00000001       | 10000001       | Test Estate 1 |
	| Safaricom    | Test Merchant 2 | 00000002       | 10000002       | Test Estate 1 |
	| Voucher      | Test Merchant 2 | 00000002       | 10000002       | Test Estate 1 |
	| Safaricom    | Test Merchant 3 | 00000003       | 10000003       | Test Estate 2 |
	| Voucher      | Test Merchant 3 | 00000003       | 10000003       | Test Estate 2 |

	Given I have created the following security users
	| EmailAddress                  | Password | GivenName    | FamilyName | EstateName    | MerchantName    |
	| merchantuser@testmerchant1.co.uk | 123456   | TestMerchant | User1      | Test Estate 1 | Test Merchant 1 |
	| merchantuser@testmerchant2.co.uk | 123456   | TestMerchant | User2      | Test Estate 1 | Test Merchant 2 |
	| merchantuser@testmerchant3.co.uk | 123456   | TestMerchant | User3      | Test Estate 2 | Test Merchant 3 |

	Given I create a contract with the following values
	| EstateName    | OperatorName | ContractDescription |
	| Test Estate 1 | Safaricom    | Safaricom Contract  |
	| Test Estate 1 | Voucher      | Hospital 1 Contract |
	| Test Estate 2 | Safaricom    | Safaricom Contract  |
	| Test Estate 2 | Voucher      | Hospital 1 Contract |

	When I create the following Products
	| EstateName    | OperatorName | ContractDescription | ProductName    | DisplayText | Value | ProductType |
	| Test Estate 1 | Safaricom    | Safaricom Contract  | Variable Topup | Custom      |       | MobileTopup |
	| Test Estate 1 | Voucher      | Hospital 1 Contract | 10 KES         | 10 KES      |       | Voucher     |
	| Test Estate 2 | Safaricom    | Safaricom Contract  | Variable Topup | Custom      |       | MobileTopup |
	| Test Estate 2 | Voucher      | Hospital 1 Contract | 10 KES         | 10 KES      |       | Voucher     |

	When I add the following Transaction Fees
	| EstateName    | OperatorName | ContractDescription | ProductName    | CalculationType | FeeDescription      | Value |
	| Test Estate 1 | Safaricom    | Safaricom Contract  | Variable Topup | Fixed           | Merchant Commission | 2.50  |
	| Test Estate 2 | Safaricom    | Safaricom Contract  | Variable Topup | Percentage      | Merchant Commission | 0.85  |

	When I add the following contracts to the following merchants
	| EstateName    | MerchantName    | ContractDescription       |
	| Test Estate 1 | Test Merchant 1 | Safaricom Contract        |
	| Test Estate 1 | Test Merchant 1 | Hospital 1 Contract       |
	| Test Estate 1 | Test Merchant 2 | Safaricom Contract        |
	| Test Estate 1 | Test Merchant 2 | Hospital 1 Contract       |
	| Test Estate 2 | Test Merchant 3 | Safaricom Contract        |
	| Test Estate 2 | Test Merchant 3 | Hospital 1 Contract       |


Scenario: Get Merchant
	Given I am logged in as "merchantuser@testmerchant1.co.uk" with password "123456" for Merchant "Test Merchant 1" for Estate "Test Estate 1" with client "merchantClient"
	When I get the merchant information for Merchant "Test Merchant 1" for Estate "Test Estate 1" the response should contain the following information
	| MerchantName   | AddressLine1   | Town     | Region      | Country        | ContactName    | EmailAddress                 |
	| Test Merchant 1 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 1 | testcontact1@merchant1.co.uk |

Scenario: Get Merchant Contracts
	Given I am logged in as "merchantuser@testmerchant1.co.uk" with password "123456" for Merchant "Test Merchant 1" for Estate "Test Estate 1" with client "merchantClient"
	When I get the merchant contract information for Merchant "Test Merchant 1" for Estate "Test Estate 1" the response should contain the following information
	| MerchantName    | ContractDescription |
	| Test Merchant 1 | Safaricom Contract  |
	| Test Merchant 1 | Hospital 1 Contract |