namespace TransactionProcessorACL.BusinessLogic.Requests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using MediatR;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest{TransactionProcessorACL.Models.ProcessReconciliationResponse}" />
    /// <seealso cref="MediatR.IRequest{System.Object}" />
    public class ProcessReconciliationRequest : IRequest<ProcessReconciliationResponse>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessReconciliationRequest"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ProcessReconciliationRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessReconciliationRequest" /> class.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="transactionCount">The transaction count.</param>
        /// <param name="transactionValue">The transaction value.</param>
        private ProcessReconciliationRequest(Guid estateId,
                                             Guid merchantId,
                                             DateTime transactionDateTime,
                                             String deviceIdentifier,
                                             Int32 transactionCount,
                                             Decimal transactionValue)
        {
            this.EstateId = estateId;
            this.MerchantId = merchantId;
            this.DeviceIdentifier = deviceIdentifier;
            this.TransactionCount = transactionCount;
            this.TransactionValue = transactionValue;
            this.TransactionDateTime = transactionDateTime;
        }

        #endregion

        #region Properties

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
        /// Gets the transaction count.
        /// </summary>
        /// <value>
        /// The transaction count.
        /// </value>
        public Int32 TransactionCount { get; }

        /// <summary>
        /// Gets the transaction date time.
        /// </summary>
        /// <value>
        /// The transaction date time.
        /// </value>
        public DateTime TransactionDateTime { get; }

        /// <summary>
        /// Gets the transaction value.
        /// </summary>
        /// <value>
        /// The transaction value.
        /// </value>
        public Decimal TransactionValue { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified estate identifier.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="transactionCount">The transaction count.</param>
        /// <param name="transactionValue">The transaction value.</param>
        /// <returns></returns>
        public static ProcessReconciliationRequest Create(Guid estateId,
                                                          Guid merchantId,
                                                          DateTime transactionDateTime,
                                                          String deviceIdentifier,
                                                          Int32 transactionCount,
                                                          Decimal transactionValue)
        {
            return new ProcessReconciliationRequest(estateId, merchantId, transactionDateTime, deviceIdentifier, transactionCount, transactionValue);
        }

        #endregion
    }
}