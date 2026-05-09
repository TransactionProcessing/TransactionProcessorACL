using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.DataTransferObjects.Responses
{
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TransactionResponseMessage
    {
        public String ResponseCode { get; set; }

        public String ResponseMessage { get; set; }

        public Dictionary<String, String> AdditionalResponseMetadata { get; set; }

        public Guid EstateId { get; set; }

        public Guid MerchantId { get; set; }

        public Boolean RequiresApplicationUpdate { get; set; }

        public Guid TransactionId { get; set; }
    }
}
