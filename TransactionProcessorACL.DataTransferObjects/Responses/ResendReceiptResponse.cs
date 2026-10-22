using System.ComponentModel;

namespace TransactionProcessorACL.DataTransferObjects.Responses;

public class ResendReceiptResponse
{
    [Description("True when the receipt resend request succeeded.")]
    public bool Success { get; set; }

    [Description("Human-readable result or failure reason.")]
    public string Message { get; set; }

    [Description("Transaction or receipt reference supplied with the request.")]
    public string Reference { get; set; }

    [Description("Receipt reference associated with the resent receipt, when available.")]
    public string ReceiptReference { get; set; }

    [Description("Transaction reference associated with the resent receipt, when available.")]
    public string TransactionReference { get; set; }
}
