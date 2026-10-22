namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TransactionRequestMessage
    {
        #region Properties

        [Description("Estate identifier from the transaction context. The API derives the authoritative estate from the access token.")]
        public Guid EstateId { get; set; }
        [Description("Merchant identifier from the transaction context. The API derives the authoritative merchant from the access token.")]
        public Guid MerchantId { get; set; }

        [Description("Client application version. The API version check is performed using the applicationVersion query parameter or the application_version JSON field.")]
        public String ApplicationVersion { get; set; }

        [Description("Identifier of the device that initiated the transaction.")]
        public String DeviceIdentifier { get; set; }

        [Description("Date and time when the transaction occurred.")]
        public DateTime TransactionDateTime { get; set; }

        [Description("Client supplied transaction reference or number.")]
        public String TransactionNumber { get; set; }

        [Description("Optional string key/value metadata associated with the request.")]
        public Dictionary<String,String> AdditionalRequestMetadata { get; set; }

        #endregion
    }
}
