namespace TransactionProcessorACL.DataTransferObjects.Responses{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;

    [ExcludeFromCodeCoverage]
    public class GetVoucherResponseMessage{
        #region Properties

        [Description("Application response code for the voucher lookup.")]
        public String ResponseCode{ get; set; }

        [Description("Human-readable result message.")]
        public String ResponseMessage{ get; set; }

        [Description("Contract that issued the voucher.")]
        public Guid ContractId{ get; set; }

        [Description("Estate that owns the voucher.")]
        public Guid EstateId{ get; set; }

        [Description("Voucher expiry date and time.")]
        public DateTime ExpiryDate{ get; set; }

        [Description("Original value of the voucher.")]
        public Decimal Value{ get; set; }

        [Description("Remaining redeemable balance.")]
        public Decimal Balance{ get; set; }

        [Description("Voucher code returned by the lookup.")]
        public String VoucherCode{ get; set; }

        [Description("Unique voucher identifier.")]
        public Guid VoucherId{ get; set; }

        #endregion
    }
}
