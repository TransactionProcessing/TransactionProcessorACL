namespace TransactionProcessorACL.Common.Examples;

using System.Diagnostics.CodeAnalysis;
using DataTransferObjects.Responses;
using Swashbuckle.AspNetCore.Filters;

[ExcludeFromCodeCoverage]
public class GetVoucherResponseMessageExample : IExamplesProvider<GetVoucherResponseMessage>
{
    #region Methods

    /// <summary>
    /// Gets the examples.
    /// </summary>
    /// <returns></returns>
    public GetVoucherResponseMessage GetExamples()
    {
        return new GetVoucherResponseMessage
               {
                   Balance = ExampleData.Balance,
                   ContractId = ExampleData.ContractId,
                   EstateId = ExampleData.EstateId,
                   ExpiryDate = ExampleData.ExpiryDate,
                   ResponseCode = ExampleData.ResponseCode,
                   ResponseMessage = ExampleData.ResponseMessage,
                   Value = ExampleData.Value,
                   VoucherCode = ExampleData.VoucherCode,
                   VoucherId = ExampleData.VoucherId
               };
    }

    #endregion
}