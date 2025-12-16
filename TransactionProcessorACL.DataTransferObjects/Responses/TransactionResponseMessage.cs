using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.DataTransferObjects.Responses
{
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    public class TransactionResponseMessage
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>
        /// The response code.
        /// </value>
        [JsonProperty("response_code")]
        public String ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        [JsonProperty("response_message")]
        public String ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the additional response meta data.
        /// </summary>
        /// <value>
        /// The additional response meta data.
        /// </value>
        [JsonProperty("additional_response_metadata")]
        public Dictionary<String, String> AdditionalResponseMetadata { get; set; }

        /// <summary>
        /// Gets or sets the estate identifier.
        /// </summary>
        /// <value>
        /// The estate identifier.
        /// </value>
        [JsonProperty("estate_id")]
        public Guid EstateId { get; set; }

        /// <summary>
        /// Gets or sets the merchant identifier.
        /// </summary>
        /// <value>
        /// The merchant identifier.
        /// </value>
        [JsonProperty("merchant_id")]
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [requires application update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires application update]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("requires_application_update")]
        public Boolean RequiresApplicationUpdate { get; set; }

        [JsonProperty("transaction_id")]
        public Guid TransactionId { get; set; }
    }
}
