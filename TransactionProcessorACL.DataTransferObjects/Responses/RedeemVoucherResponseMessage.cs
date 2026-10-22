namespace TransactionProcessorACL.DataTransferObjects.Responses{

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    
    [ExcludeFromCodeCoverage]
    public class RedeemVoucherResponseMessage{
        #region Properties

        [Description("Application response code for the redemption.")]
        public String ResponseCode{ get; set; }

        [Description("Human-readable result message.")]
        public String ResponseMessage{ get; set; }

        [Description("Contract that issued the voucher.")]
        public Guid ContractId{ get; set; }

        [Description("Estate that owns the voucher.")]
        public Guid EstateId{ get; set; }

        [Description("Voucher expiry date and time.")]
        public DateTime ExpiryDate{ get; set; }

        [Description("Remaining balance after redemption.")]
        public Decimal Balance{ get; set; }

        [Description("Redeemed voucher code.")]
        public String VoucherCode{ get; set; }

        #endregion
    }

    [ExcludeFromCodeCoverage]
    public class MerchantScheduleResponse
    {
        [Description("Calendar year covered by the schedule.")]
        public int Year { get; set; }

        [Description("Schedule details grouped by calendar month.")]
        public List<MerchantScheduleMonthResponse> Months { get; set; } = new List<MerchantScheduleMonthResponse>();
    }

    [ExcludeFromCodeCoverage]
    public class MerchantScheduleMonthResponse
    {
        [Description("Calendar month number, from 1 (January) through 12 (December).")]
        public int Month { get; set; }

        [Description("Day numbers in this month when the merchant is closed.")]
        public List<int> ClosedDays { get; set; } = new List<int>();
    }
}
