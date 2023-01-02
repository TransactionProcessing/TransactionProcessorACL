namespace TransactionProcessorACL.Common.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using DataTransferObjects.Responses;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.Filters.IMultipleExamplesProvider{TransactionProcessorACL.DataTransferObjects.Responses.TransactionResponseMessage}" />
    [ExcludeFromCodeCoverage]
    public class TransactionResponseMessageExample : IMultipleExamplesProvider<TransactionResponseMessage>
    {
        #region Methods

        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SwaggerExample<TransactionResponseMessage>> GetExamples()
        {
            return new List<SwaggerExample<TransactionResponseMessage>>
                   {
                       new SwaggerExample<TransactionResponseMessage>
                       {
                           Name = "Logon Transaction",
                           Value = new LogonTransactionResponseMessage
                                   {
                                       AdditionalResponseMetaData = new Dictionary<String, String>(),
                                       EstateId = ExampleData.EstateId,
                                       MerchantId = ExampleData.MerchantId,
                                       RequiresApplicationUpdate = ExampleData.RequiresApplicationUpdate,
                                       ResponseCode = ExampleData.ResponseCode,
                                       ResponseMessage = ExampleData.ResponseMessage
                                   }
                       },
                       new SwaggerExample<TransactionResponseMessage>
                       {
                           Name = "Sale Transaction",
                           Value = new SaleTransactionResponseMessage
                                   {
                                       AdditionalResponseMetaData = new Dictionary<String, String>(),
                                       EstateId = ExampleData.EstateId,
                                       MerchantId = ExampleData.MerchantId,
                                       RequiresApplicationUpdate = ExampleData.RequiresApplicationUpdate,
                                       ResponseCode = ExampleData.ResponseCode,
                                       ResponseMessage = ExampleData.ResponseMessage
                                   }
                       },
                       new SwaggerExample<TransactionResponseMessage>
                       {
                           Name = "Reconciliation Transaction",
                           Value = new ReconciliationResponseMessage
                                   {
                                       AdditionalResponseMetaData = new Dictionary<String, String>(),
                                       EstateId = ExampleData.EstateId,
                                       MerchantId = ExampleData.MerchantId,
                                       RequiresApplicationUpdate = ExampleData.RequiresApplicationUpdate,
                                       ResponseCode = ExampleData.ResponseCode,
                                       ResponseMessage = ExampleData.ResponseMessage
                                   }
                       },
                   };
        }

        #endregion
    }
}