namespace TransactionProcessorACL.DataTransferObjects.Responses{

    using System;
    using System.Collections.Generic;
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

    [ExcludeFromCodeCoverage]
    public class MerchantScheduleResponse
    {
        public int Year { get; set; }

        public List<MerchantScheduleMonthResponse> Months { get; set; } = new List<MerchantScheduleMonthResponse>();
    }

    [ExcludeFromCodeCoverage]
    public class MerchantScheduleMonthResponse
    {
        public int Month { get; set; }

        public List<int> ClosedDays { get; set; } = new List<int>();
    }
}