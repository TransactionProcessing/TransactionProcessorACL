﻿namespace TransactionProcessorACL.BusinessLogic.Requests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using MediatR;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest{System.Object}" />
    public class ProcessLogonTransactionRequest : IRequest<ProcessLogonTransactionResponse>
    {
        #region Constructors

        [ExcludeFromCodeCoverage]
        public ProcessLogonTransactionRequest()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLogonTransactionRequest" /> class.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="requireConfigurationInResponse">if set to <c>true</c> [require configuration in response].</param>
        private ProcessLogonTransactionRequest(Guid estateId,
                                               Guid merchantId,
                                               DateTime transactionDateTime,
                                               String transactionNumber,
                                               String deviceIdentifier)
        {
            this.EstateId = estateId;
            this.MerchantId = merchantId;
            this.DeviceIdentifier = deviceIdentifier;
            this.TransactionDateTime = transactionDateTime;
            this.TransactionNumber = transactionNumber;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the estate identifier.
        /// </summary>
        /// <value>
        /// The estate identifier.
        /// </value>
        public Guid EstateId { get; }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public String DeviceIdentifier { get; }

        /// <summary>
        /// Gets the merchant identifier.
        /// </summary>
        /// <value>
        /// The merchant identifier.
        /// </value>
        public Guid MerchantId { get; }

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
        /// <returns></returns>
        public static ProcessLogonTransactionRequest Create(Guid estateId,
                                                            Guid merchantId,
                                                            DateTime transactionDateTime,
                                                            String transactionNumber,
                                                            String deviceIdentifier)
        {
            return new ProcessLogonTransactionRequest(estateId, merchantId, transactionDateTime, transactionNumber, deviceIdentifier);
        }

        #endregion
    }
}