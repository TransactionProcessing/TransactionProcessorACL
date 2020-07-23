namespace TransactionProcessorACL.BusinessLogic.Requests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using MediatR;
    using Models;

    public class ProcessSaleTransactionRequest : IRequest<ProcessSaleTransactionResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSaleTransactionRequest"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ProcessSaleTransactionRequest()
        {
            
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLogonTransactionRequest" /> class.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="operatorIdentifier">The operator identifier.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="customerAccountNumber">The customer account number.</param>
        /// <param name="customerEmailAddress">The customer email address.</param>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="">The .</param>
        private ProcessSaleTransactionRequest(Guid estateId,
                                              Guid merchantId,
                                              DateTime transactionDateTime,
                                              String transactionNumber,
                                              String deviceIdentifier,
                                              String operatorIdentifier,
                                              Decimal amount,
                                              String customerAccountNumber,
                                              String customerEmailAddress,
                                              Guid contractId,
                                              Guid productId)
        {
            this.EstateId = estateId;
            this.MerchantId = merchantId;
            this.DeviceIdentifier = deviceIdentifier;
            this.OperatorIdentifier = operatorIdentifier;
            this.Amount = amount;
            this.CustomerAccountNumber = customerAccountNumber;
            this.CustomerEmailAddress = customerEmailAddress;
            this.ContractId = contractId;
            this.ProductId = productId;
            this.TransactionDateTime = transactionDateTime;
            this.TransactionNumber = transactionNumber;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public Decimal Amount { get; }

        /// <summary>
        /// Gets the customer account number.
        /// </summary>
        /// <value>
        /// The customer account number.
        /// </value>
        public String CustomerAccountNumber { get; }

        /// <summary>
        /// Gets the customer email address.
        /// </summary>
        /// <value>
        /// The customer email address.
        /// </value>
        public String CustomerEmailAddress { get; private set; }

        /// <summary>
        /// Gets the contract identifier.
        /// </summary>
        /// <value>
        /// The contract identifier.
        /// </value>
        public Guid ContractId { get; }

        /// <summary>
        /// Gets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public Guid ProductId { get; }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public String DeviceIdentifier { get; }

        /// <summary>
        /// Gets the estate identifier.
        /// </summary>
        /// <value>
        /// The estate identifier.
        /// </value>
        public Guid EstateId { get; }

        /// <summary>
        /// Gets the merchant identifier.
        /// </summary>
        /// <value>
        /// The merchant identifier.
        /// </value>
        public Guid MerchantId { get; }

        /// <summary>
        /// Gets the operator identifier.
        /// </summary>
        /// <value>
        /// The operator identifier.
        /// </value>
        public String OperatorIdentifier { get; }

        /// <summary>
        /// Gets the transaction date time.
        /// </summary>
        /// <value>
        /// The transaction date time.
        /// </value>
        public DateTime TransactionDateTime { get; }

        /// <summary>
        /// Gets the transaction number.
        /// </summary>
        /// <value>
        /// The transaction number.
        /// </value>
        public String TransactionNumber { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified estate identifier.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="operatorIdentifier">The operator identifier.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="customerAccountNumber">The customer account number.</param>
        /// <param name="customerEmailAddress">The customer email address.</param>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        public static ProcessSaleTransactionRequest Create(Guid estateId,
                                                           Guid merchantId,
                                                           DateTime transactionDateTime,
                                                           String transactionNumber,
                                                           String deviceIdentifier,
                                                           String operatorIdentifier,
                                                           Decimal amount,
                                                           String customerAccountNumber,
                                                           String customerEmailAddress,
                                                           Guid contractId,
                                                           Guid productId)
        {
            return new ProcessSaleTransactionRequest(estateId,
                                                     merchantId,
                                                     transactionDateTime,
                                                     transactionNumber,
                                                     deviceIdentifier,
                                                     operatorIdentifier,
                                                     amount,
                                                     customerAccountNumber,
                                                     customerEmailAddress,
                                                     contractId,
                                                     productId);
        }

        #endregion
    }
}