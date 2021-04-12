namespace TransactionProcessorACL.Common.Examples
{
    using System;
    using System.Collections.Generic;
    using DataTransferObjects;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.Filters.IMultipleExamplesProvider{TransactionProcessorACL.DataTransferObjects.TransactionRequestMessage}" />
    public class TransactionRequestMessageExample : IMultipleExamplesProvider<TransactionRequestMessage>
    {
        #region Methods

        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SwaggerExample<TransactionRequestMessage>> GetExamples()
        {
            return new List<SwaggerExample<TransactionRequestMessage>>
                   {
                       new SwaggerExample<TransactionRequestMessage>
                       {
                           Name = "Logon Transaction",
                           Value = new LogonTransactionRequestMessage
                                   {
                                       AdditionalRequestMetaData = new Dictionary<String, String>(),
                                       ApplicationVersion = ExampleData.ApplicationVersion,
                                       DeviceIdentifier = ExampleData.DeviceIdentifier,
                                       TransactionDateTime = ExampleData.TransactionDateTime,
                                       TransactionNumber = ExampleData.TransactionNumber
                                   }
                       },
                       new SwaggerExample<TransactionRequestMessage>
                       {
                           Name = "Sale Transaction",
                           Value = new SaleTransactionRequestMessage
                                   {
                                       AdditionalRequestMetaData = new Dictionary<String, String>(),
                                       ApplicationVersion = ExampleData.ApplicationVersion,
                                       DeviceIdentifier = ExampleData.DeviceIdentifier,
                                       TransactionDateTime = ExampleData.TransactionDateTime,
                                       TransactionNumber = ExampleData.TransactionNumber,
                                       ContractId = ExampleData.ContractId,
                                       CustomerEmailAddress = ExampleData.CustomerEmailAddress,
                                       OperatorIdentifier = ExampleData.OperatorIdentifier,
                                       ProductId = ExampleData.ProductId
                                   }
                       },
                       new SwaggerExample<TransactionRequestMessage>
                       {
                           Name = "Reconciliation Transaction",
                           Value = new ReconciliationRequestMessage
                                   {
                                       AdditionalRequestMetaData = new Dictionary<String, String>(),
                                       ApplicationVersion = ExampleData.ApplicationVersion,
                                       DeviceIdentifier = ExampleData.DeviceIdentifier,
                                       TransactionDateTime = ExampleData.TransactionDateTime,
                                       TransactionNumber = ExampleData.TransactionNumber,
                                       OperatorTotals = new List<OperatorTotalRequest>
                                                        {
                                                            new OperatorTotalRequest
                                                            {
                                                                ContractId = ExampleData.ContractId,
                                                                OperatorIdentifier = ExampleData.OperatorIdentifier,
                                                                TransactionCount = ExampleData.TransactionCount,
                                                                TransactionValue = ExampleData.TransactionValue
                                                            }
                                                        },
                                       TransactionCount = ExampleData.TransactionCount,
                                       TransactionValue = ExampleData.TransactionValue
                                   }
                       }
                   };
        }

        #endregion
    }
}