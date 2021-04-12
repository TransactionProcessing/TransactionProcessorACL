namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TransactionRequestMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        [JsonProperty("application_version")]
        public String ApplicationVersion { get; set; }

        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        [JsonProperty("device_identifier")]
        public String DeviceIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the transaction date time.
        /// </summary>
        /// <value>
        /// The transaction date time.
        /// </value>
        [JsonProperty("transaction_date_time")]
        public DateTime TransactionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the transaction number.
        /// </summary>
        /// <value>
        /// The transaction number.
        /// </value>
        [JsonProperty("transaction_number")]
        public String TransactionNumber { get; set; }

        /// <summary>
        /// Gets or sets the additional request meta data.
        /// </summary>
        /// <value>
        /// The additional request meta data.
        /// </value>
        [JsonProperty("additional_request_metadata")]
        public Dictionary<String,String> AdditionalRequestMetaData { get; set; }

        #endregion
    }
}