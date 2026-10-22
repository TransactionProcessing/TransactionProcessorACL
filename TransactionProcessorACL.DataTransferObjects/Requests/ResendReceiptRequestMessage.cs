using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class ResendReceiptRequestMessage
{
    [Description("Transaction reference or receipt reference for the receipt to resend. Required.")]
    public string Reference { get; set; }

    [Description("Valid email address to receive the receipt. Required.")]
    public string RecipientEmailAddress { get; set; }
}
