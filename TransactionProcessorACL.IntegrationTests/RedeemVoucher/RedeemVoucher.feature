﻿@base @shared
Feature: RedeemVoucher

Background: 

	Given the following security roles exist
	| Role Name |
	| Merchant   |
	| VoucherRedemption   |

	Given I create the following api scopes
	| Name                 | DisplayName                       | Description                            |
	| estateManagement     | Estate Managememt REST Scope      | A scope for Estate Managememt REST     |
	| transactionProcessor | Transaction Processor REST  Scope | A scope for Transaction Processor REST |
	| transactionProcessorACL | Transaction Processor ACL REST  Scope | A scope for Transaction Processor ACL REST |
	| voucherManagement | Voucher Management REST  Scope | A scope for Voucher Management REST |

	Given the following api resources exist
	| Name            | DisplayName                    | Secret  | Scopes                  | UserClaims                 |
	| estateManagement        | Estate Managememt REST         | Secret1 | estateManagement        | merchantId, estateId, role |
	| transactionProcessor    | Transaction Processor REST     | Secret1 | transactionProcessor    |                            |
	| transactionProcessorACL | Transaction Processor ACL REST | Secret1 | transactionProcessorACL | merchantId, estateId, role, contractId |
	| voucherManagement       | Voucher Management REST        | Secret1 | voucherManagement       |                            |

	Given the following clients exist
	| ClientId         | ClientName        | Secret  | Scopes                                                                   | GrantTypes  |
	| serviceClient    | Service Client    | Secret1 | estateManagement,transactionProcessor,transactionProcessorACL,voucherManagement | client_credentials |
	| merchantClient   | Merchant Client   | Secret1 | transactionProcessorACL                                                         | password           |
	| redemptionClient | Redemption Client | Secret1 | transactionProcessorACL                                                         | password           |

	Given I have a token to access the estate management and transaction processor acl resources
	| ClientId      | 
	| serviceClient | 

	Given I have created the following estates
	| EstateName    |
	| Test Estate 1 |
	| Test Estate 2 |

	Given I have created the following security users for voucher redemption
	| Email Address                         | Password | Given Name      | Family Name | EstateName    | Roles          |
	| redemptionuser@testredemption1.co.uk | 123456   | TestRedemption | User1      | Test Estate 1 | VoucherRedemption |
	| redemptionuser@testredemption2.co.uk | 123456   | TestRedemption | User2      | Test Estate 2 | VoucherRedemption |

	Given I have created the following operators
	| EstateName    | OperatorName | RequireCustomMerchantNumber | RequireCustomTerminalNumber |
	| Test Estate 1 | Voucher      | True                        | True                        |
	| Test Estate 2 | Voucher      | True                        | True                        |

	And I have assigned the following operators to the estates
	| EstateName    | OperatorName    | 
	| Test Estate 1 | Voucher |
	| Test Estate 2 | Voucher |

	Given I create a contract with the following values
	| EstateName    | OperatorName    | ContractDescription |
	| Test Estate 1 | Voucher          | Hospital 1 Contract       |
	| Test Estate 2 | Voucher          | Hospital 2 Contract       |

	When I create the following Products
	| EstateName    | OperatorName | ContractDescription | ProductName    | DisplayText | Value | ProductType |
	| Test Estate 1 | Voucher      | Hospital 1 Contract | 10 KES      | 10 KES      |       | Voucher     |
	| Test Estate 2 | Voucher      | Hospital 2 Contract | 10 KES      | 10 KES      |       | Voucher     |

	Given I create the following merchants
	| MerchantName    | AddressLine1   | Town     | Region      | Country        | ContactName    | EmailAddress                 | EstateName    |
	| Test Merchant 1 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 1 | testcontact1@merchant1.co.uk | Test Estate 1 |
	| Test Merchant 2 | Address Line 1 | TestTown | Test Region | United Kingdom | Test Contact 1 | testcontact1@merchant1.co.uk | Test Estate 2 |

	Given I have assigned the following  operator to the merchants
	| OperatorName | MerchantName    | MerchantNumber | TerminalNumber | EstateName    |
	| Voucher          | Test Merchant 1 | 00000001       | 10000001       | Test Estate 1 |
	| Voucher          | Test Merchant 2 | 00000002       | 10000002       | Test Estate 2 |

	Given I have assigned the following devices to the merchants
	| DeviceIdentifier | MerchantName    | EstateName    |
	| 123456780        | Test Merchant 1 | Test Estate 1 |
	| 123456781        | Test Merchant 2 | Test Estate 2 |

When I add the following contracts to the following merchants
	| EstateName    | MerchantName    | ContractDescription       |
	| Test Estate 1 | Test Merchant 1 | Hospital 1 Contract       |
	| Test Estate 2 | Test Merchant 2 | Hospital 2 Contract       |

	Given I make the following manual merchant deposits 
	| Reference | Amount  | DateTime | MerchantName    | EstateName    |
	| Deposit1  | 20.00 | Today    | Test Merchant 1 | Test Estate 1 |
	| Deposit1  | 20.00 | Today    | Test Merchant 2 | Test Estate 2 |

	Given I have created the following security users
	| EmailAddress                     | Password | GivenName    | FamilyName | EstateName    | MerchantName    |
	| merchantuser@testmerchant1.co.uk | 123456   | TestMerchant | User1      | Test Estate 1 | Test Merchant 1 |
	| merchantuser@testmerchant2.co.uk | 123456   | TestMerchant | User2      | Test Estate 2 | Test Merchant 2 |

	Given I am logged in as "merchantuser@testmerchant1.co.uk" with password "123456" for Merchant "Test Merchant 1" for Estate "Test Estate 1" with client "merchantClient"

	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | TransactionSource | MerchantName    | DeviceIdentifier | EstateName    | OperatorName     | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress        | ContractDescription       | ProductName       | RecipientEmail       | RecipientMobile | MessageType   | AccountNumber | CustomerName     |
	| Today    | 1                 | Sale            | 1                 | Test Merchant 1 | 123456780        | Test Estate 1 | Voucher          | 10.00             |                       |                             | Hospital 1 Contract       | 10 KES            | test@recipient.co.uk |                 |               |               |                  |

	Given I am logged in as "merchantuser@testmerchant2.co.uk" with password "123456" for Merchant "Test Merchant 2" for Estate "Test Estate 2" with client "merchantClient"

	When I perform the following transactions
	| DateTime | TransactionNumber | TransactionType | TransactionSource | MerchantName    | DeviceIdentifier | EstateName    | OperatorName     | TransactionAmount | CustomerAccountNumber | CustomerEmailAddress        | ContractDescription       | ProductName       | RecipientEmail       | RecipientMobile | MessageType   | AccountNumber | CustomerName     |
	| Today    | 2                 | Sale            | 1                 | Test Merchant 2 | 123456781        | Test Estate 2 | Voucher          | 10.00             |                       |                             | Hospital 2 Contract       | 10 KES            | test@recipient.co.uk |                 |               |               |                  |

@PRTest
Scenario: Redeem Vouchers
	Given I am logged in as "redemptionuser@testredemption1.co.uk" with password "123456" for Estate "Test Estate 1" with client "redemptionClient"

	When I redeem the voucher for Estate 'Test Estate 1' and Merchant 'Test Merchant 1' transaction number 1 the voucher balance will be 0

	Given I am logged in as "redemptionuser@testredemption2.co.uk" with password "123456" for Estate "Test Estate 2" with client "redemptionClient"

	When I redeem the voucher for Estate 'Test Estate 2' and Merchant 'Test Merchant 2' transaction number 2 the voucher balance will be 0
