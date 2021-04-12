using System;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionProcessorACL.Common.Examples
{
    using Microsoft.Extensions.Primitives;
    using TransactionProcessor.DataTransferObjects;

    /// <summary>
    /// 
    /// </summary>
    internal static class ExampleData
    {
        /// <summary>
        /// The transaction count
        /// </summary>
        internal static Int32 TransactionCount = 1;
        /// <summary>
        /// The transaction value
        /// </summary>
        internal static Decimal TransactionValue = 10.00m;

        /// <summary>
        /// The application version
        /// </summary>
        internal static String ApplicationVersion = "1.0.0";

        /// <summary>
        /// The device identifier
        /// </summary>
        internal static String DeviceIdentifier = "exampledevice1";

        /// <summary>
        /// The transaction date time
        /// </summary>
        internal static DateTime TransactionDateTime = new DateTime(2021, 4, 9, 11, 3, 0);

        /// <summary>
        /// The transaction number
        /// </summary>
        internal static String TransactionNumber = "0001";
        /// <summary>
        /// The contract identifier
        /// </summary>
        internal static Guid ContractId = Guid.Parse("F969AAAB-A669-4EC2-A060-62082DD19096");

        /// <summary>
        /// The customer email address
        /// </summary>
        internal static String CustomerEmailAddress = "exampleemail@customerdomain.co.uk";

        /// <summary>
        /// The operator identifier
        /// </summary>
        internal static String OperatorIdentifier = "Safaricom";
        /// <summary>
        /// The product identifier
        /// </summary>
        internal static Guid ProductId = Guid.Parse("743846D5-2FDC-47F7-8525-5D366BC5F67E");

        /// <summary>
        /// The estate identifier
        /// </summary>
        internal static Guid EstateId = Guid.Parse("D0BF0236-4709-4262-BD2C-3C8AF16189C9");
        /// <summary>
        /// The merchant identifier
        /// </summary>
        internal static Guid MerchantId = Guid.Parse("D0BF0236-4709-4262-BD2C-3C8AF16189C9");

        /// <summary>
        /// The response code
        /// </summary>
        internal static String ResponseCode = "0000";
        /// <summary>
        /// The response message
        /// </summary>
        internal static String ResponseMessage = "SUCCESS";

        /// <summary>
        /// The requires application update
        /// </summary>
        internal static Boolean RequiresApplicationUpdate = false;
    }
}
