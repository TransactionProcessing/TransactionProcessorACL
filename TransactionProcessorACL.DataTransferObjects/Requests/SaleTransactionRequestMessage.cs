namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.DataTransferObjects.TransactionRequestMessage" />
    [ExcludeFromCodeCoverage]
    public class SaleTransactionRequestMessage : TransactionRequestMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the contract identifier.
        /// </summary>
        /// <value>
        /// The contract identifier.
        /// </value>
        [JsonProperty("contract_id")]
        public Guid ContractId { get; set; }

        /// <summary>
        /// Gets or sets the operator identifier.
        /// </summary>
        /// <value>
        /// The operator identifier.
        /// </value>
        [JsonProperty("operator_identifier")]
        public String OperatorIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        /// <value>
        /// The customer email address.
        /// </value>
        [JsonProperty("customer_email_address")]
        public String CustomerEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }

        #endregion
    }
}