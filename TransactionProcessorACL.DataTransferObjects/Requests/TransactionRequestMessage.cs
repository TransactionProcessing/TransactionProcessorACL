namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TransactionRequestMessage
    {
        #region Properties

        public Guid EstateId { get; set; }
        public Guid MerchantId { get; set; }

        public String ApplicationVersion { get; set; }

        public String DeviceIdentifier { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public String TransactionNumber { get; set; }

        public Dictionary<String,String> AdditionalRequestMetadata { get; set; }

        #endregion
    }
}