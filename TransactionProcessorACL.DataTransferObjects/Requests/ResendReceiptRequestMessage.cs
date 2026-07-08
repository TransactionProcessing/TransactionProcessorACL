using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class ResendReceiptRequestMessage
{
    public string Reference { get; set; }

    public string RecipientEmailAddress { get; set; }
}
