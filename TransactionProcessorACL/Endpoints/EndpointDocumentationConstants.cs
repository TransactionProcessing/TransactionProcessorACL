namespace TransactionProcessorACL.Endpoints;

internal static class EndpointDocumentationConstants
{
    public const string TransactionsTag = "Transactions";
    public const string ApplicationVersionQueryParameter = "applicationVersion";
    public const string ApplicationVersionExample = "1.0.5";
    public const string ApplicationVersionQueryDescription = "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.";
    public const string JsonMediaType = "application/json";
}
