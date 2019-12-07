namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.DataTransferObjects.TransactionRequestMessage" />
    [ExcludeFromCodeCoverage]
    public class LogonTransactionRequestMessage : TransactionRequestMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [require configuration in response].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require configuration in response]; otherwise, <c>false</c>.
        /// </value>
        public Boolean RequireConfigurationInResponse { get; set; }

        #endregion
    }
}