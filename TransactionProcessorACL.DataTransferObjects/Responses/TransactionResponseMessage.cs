using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.DataTransferObjects.Responses
{
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    
    [ExcludeFromCodeCoverage]
    public class TransactionResponseMessage
    {
        [Description("Application response code for the transaction result.")]
        public String ResponseCode { get; set; }

        [Description("Human-readable message describing the transaction result.")]
        public String ResponseMessage { get; set; }

        [Description("Optional string key/value metadata returned by the transaction processor.")]
        public Dictionary<String, String> AdditionalResponseMetadata { get; set; }

        [Description("Estate that owns the transaction.")]
        public Guid EstateId { get; set; }

        [Description("Merchant that owns the transaction.")]
        public Guid MerchantId { get; set; }

        [Description("Indicates whether the client must update its application before continuing.")]
        public Boolean RequiresApplicationUpdate { get; set; }

        [Description("Identifier assigned to the processed transaction.")]
        public Guid TransactionId { get; set; }
    }
}
