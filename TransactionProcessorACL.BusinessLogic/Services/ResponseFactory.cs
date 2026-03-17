using System;
using System.Collections.Generic;
using TransactionProcessorACL.BusinessLogic.Models;
using ContractContactResponse = TransactionProcessor.DataTransferObjects.Responses.Contract.ContactResponse;
using ModelContactResponse = TransactionProcessorACL.BusinessLogic.Models.ContactResponse;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    public static class ResponseFactory
    {
        public static MerchantResponse Build(TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantResponse merchant) {
            MerchantResponse merchantResponse = new() {
                MerchantId = merchant.MerchantId,
                EstateId = merchant.EstateId,
                MerchantName = merchant.MerchantName,
                EstateReportingId = merchant.EstateReportingId,
                MerchantReference = merchant.MerchantReference,
                NextStatementDate = merchant.NextStatementDate,
                SettlementSchedule = merchant.SettlementSchedule switch {
                    TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Immediate => SettlementSchedule.Immediate,
                    TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Weekly => SettlementSchedule.Weekly,
                    TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly => SettlementSchedule.Monthly,
                    _ => SettlementSchedule.NotSet
                },
                Contracts = new(),
                Contacts = new(),
                Addresses = new(),
                Devices = new(),
                Operators = new()
            };

            PopulateMerchantAddresses(merchant.Addresses, merchantResponse);
            PopulateMerchantContacts(merchant.Contacts, merchantResponse);
            PopulateMerchantContracts(merchant.Contracts, merchantResponse);
            PopulateMerchantDevices(merchant.Devices, merchantResponse);
            PopulateMerchantOperators(merchant.Operators, merchantResponse);

            return merchantResponse;
        }

        private static void PopulateMerchantAddresses(List<TransactionProcessor.DataTransferObjects.Responses.Merchant.AddressResponse> addresses,
                                                      MerchantResponse merchantResponse) {
            if (addresses == null) {
                return;
            }

            foreach (TransactionProcessor.DataTransferObjects.Responses.Merchant.AddressResponse address in addresses) {
                AddressResponse addressResponse = new() {
                    AddressId = address.AddressId,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    AddressLine3 = address.AddressLine3,
                    AddressLine4 = address.AddressLine4,
                    Country = address.Country,
                    PostalCode = address.PostalCode,
                    Region = address.Region,
                    Town = address.Town
                };
                merchantResponse.Addresses.Add(addressResponse);
            }
        }

        private static void PopulateMerchantContacts(List<ContractContactResponse> contacts,
                                                     MerchantResponse merchantResponse) {
            if (contacts == null) {
                return;
            }

            foreach (ContractContactResponse contact in contacts) {
                merchantResponse.Contacts.Add(new ModelContactResponse {
                    ContactId = contact.ContactId,
                    ContactName = contact.ContactName,
                    ContactPhoneNumber = contact.ContactPhoneNumber,
                    ContactEmailAddress = contact.ContactEmailAddress
                });
            }
        }

        private static void PopulateMerchantContracts(List<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantContractResponse> contracts,
                                                      MerchantResponse merchantResponse) {
            if (contracts == null) {
                return;
            }

            foreach (TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantContractResponse merchantContract in contracts) {
                MerchantContractResponse contract = new() {
                    ContractId = merchantContract.ContractId,
                    IsDeleted = merchantContract.IsDeleted,
                    ContractProducts = merchantContract.ContractProducts == null ? new List<Guid>() : new List<Guid>(merchantContract.ContractProducts)
                };

                merchantResponse.Contracts.Add(contract);
            }
        }

        private static void PopulateMerchantDevices(Dictionary<Guid, string> devices,
                                                    MerchantResponse merchantResponse) {
            if (devices == null) {
                return;
            }

            foreach (KeyValuePair<Guid, string> device in devices) {
                merchantResponse.Devices.Add(device.Key, device.Value);
            }
        }

        private static void PopulateMerchantOperators(List<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantOperatorResponse> operators,
                                                      MerchantResponse merchantResponse) {
            if (operators == null) {
                return;
            }

            foreach (TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantOperatorResponse merchantOperator in operators) {
                merchantResponse.Operators.Add(new MerchantOperatorResponse {
                    OperatorId = merchantOperator.OperatorId,
                    IsDeleted = merchantOperator.IsDeleted,
                    MerchantNumber = merchantOperator.MerchantNumber,
                    Name = merchantOperator.Name,
                    TerminalNumber = merchantOperator.TerminalNumber
                });
            }
        }
    }
}
