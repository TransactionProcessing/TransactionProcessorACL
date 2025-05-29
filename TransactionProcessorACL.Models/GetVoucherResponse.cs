namespace TransactionProcessorACL.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class GetVoucherResponse
{
    #region Properties

    public String ResponseCode { get; set; }

    public String ResponseMessage { get; set; }

    public Guid ContractId { get; set; }

    public Guid EstateId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public Decimal Value { get; set; }

    public Decimal Balance { get; set; }

    public String VoucherCode { get; set; }

    public Guid VoucherId { get; set; }


    public List<String> ErrorMessages { get; set; }

    #endregion
}