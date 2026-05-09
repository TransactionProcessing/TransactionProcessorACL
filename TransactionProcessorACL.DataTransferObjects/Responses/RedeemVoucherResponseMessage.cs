namespace TransactionProcessorACL.DataTransferObjects.Responses{

    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class RedeemVoucherResponseMessage{
        #region Properties

        public String ResponseCode{ get; set; }

        public String ResponseMessage{ get; set; }

        public Guid ContractId{ get; set; }

        public Guid EstateId{ get; set; }

        public DateTime ExpiryDate{ get; set; }

        public Decimal Balance{ get; set; }

        public String VoucherCode{ get; set; }

        #endregion
    }
}