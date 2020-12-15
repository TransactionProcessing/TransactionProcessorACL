using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.DataTransferObjects.Responses
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TransactionResponseMessage
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>
        /// The response code.
        /// </value>
        public String ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        public String ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the additional response meta data.
        /// </summary>
        /// <value>
        /// The additional response meta data.
        /// </value>
        public Dictionary<String, String> AdditionalResponseMetaData { get; set; }

        /// <summary>
        /// Gets or sets the estate identifier.
        /// </summary>
        /// <value>
        /// The estate identifier.
        /// </value>
        public Guid EstateId { get; set; }

        /// <summary>
        /// Gets or sets the merchant identifier.
        /// </summary>
        /// <value>
        /// The merchant identifier.
        /// </value>
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [requires application update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires application update]; otherwise, <c>false</c>.
        /// </value>
        public Boolean RequiresApplicationUpdate { get; set; }
    }
}
