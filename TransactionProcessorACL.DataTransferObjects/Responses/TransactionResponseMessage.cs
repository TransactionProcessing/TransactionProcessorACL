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
    }
}
