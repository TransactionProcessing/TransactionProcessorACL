namespace TransactionProcessorACL.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ProcessSaleTransactionResponse
    {
        public String ResponseCode { get; set; }

        public String ResponseMessage { get; set; }

        public Guid EstateId { get; set; }

        public Guid MerchantId { get; set; }

        public Dictionary<String, String> AdditionalTransactionMetadata { get; set; }

        public Guid TransactionId { get; set; }

        public List<String> ErrorMessages { get; set; }

    }
}