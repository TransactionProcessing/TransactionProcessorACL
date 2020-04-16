namespace TransactionProcessorACL.BusinessLogic.Requests
{
    using System;
    using MediatR;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest{System.Object}" />
    public class ProcessLogonTransactionRequest : IRequest<ProcessLogonTransactionResponse>
    {
        #region Constructors

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
                                               String deviceIdentifier,
                                               Boolean requireConfigurationInResponse)
        {
            this.EstateId = estateId;
            this.MerchantId = merchantId;
            this.DeviceIdentifier = deviceIdentifier;
            this.RequireConfigurationInResponse = requireConfigurationInResponse;
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
        /// Gets a value indicating whether [require configuration in response].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require configuration in response]; otherwise, <c>false</c>.
        /// </value>
        public Boolean RequireConfigurationInResponse { get; }

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
        /// <param name="requireConfigurationInResponse">if set to <c>true</c> [require configuration in response].</param>
        /// <returns></returns>
        public static ProcessLogonTransactionRequest Create(Guid estateId,
                                                            Guid merchantId,
                                                            DateTime transactionDateTime,
                                                            String transactionNumber,
                                                            String deviceIdentifier,
                                                            Boolean requireConfigurationInResponse)
        {
            return new ProcessLogonTransactionRequest(estateId, merchantId, transactionDateTime, transactionNumber, deviceIdentifier, requireConfigurationInResponse);
        }

        #endregion
    }
}