namespace TransactionProcessorACL.Testing
{
    using System;
    using BusinessLogic.Requests;
    using DataTransferObjects;

    /// <summary>
    /// 
    /// </summary>
    public class TestData
    {
        #region Fields

        /// <summary>
        /// The estate identifier
        /// </summary>
        public static Guid EstateId = Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3");

        /// <summary>
        /// The imei number
        /// </summary>
        public static String IMEINumber = "12345678";

        /// <summary>
        /// The require configuration in response true
        /// </summary>
        public static Boolean RequireConfigurationInResponseTrue = true;

        /// <summary>
        /// The transaction date time
        /// </summary>
        public static DateTime TransactionDateTime = DateTime.Now;

        /// <summary>
        /// The transaction number
        /// </summary>
        public static String TransactionNumber = "0001";

        /// <summary>
        /// The logon transaction request message
        /// </summary>
        public static LogonTransactionRequestMessage LogonTransactionRequestMessage = new LogonTransactionRequestMessage
                                                                                      {
                                                                                          RequireConfigurationInResponse = TestData.RequireConfigurationInResponseTrue,
                                                                                          IMEINumber = TestData.IMEINumber,
                                                                                          TransactionDateTime = TestData.TransactionDateTime,
                                                                                          TransactionNumber = TestData.TransactionNumber
                                                                                      };

        /// <summary>
        /// The merchant identifier
        /// </summary>
        public static Guid MerchantId = Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3");

        /// <summary>
        /// The process logon transaction request
        /// </summary>
        public static ProcessLogonTransactionRequest ProcessLogonTransactionRequest =
            ProcessLogonTransactionRequest.Create(TestData.EstateId,
                                                  TestData.MerchantId,
                                                  TestData.TransactionDateTime,
                                                  TestData.TransactionNumber,
                                                  TestData.IMEINumber,
                                                  TestData.RequireConfigurationInResponseTrue);

        #endregion
    }
}