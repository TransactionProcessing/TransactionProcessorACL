using System;
using TransactionProcessing.UnionResults;

namespace TransactionProcessorACL.Handlers;

internal static class ResponseFactory
{
    public static IResultOutcome FromResult(Result result) => result;

    public static IResultOutcome FromResult<TSource, TTarget>(Result<TSource> result, Func<TSource, TTarget> convert)
    {
        ArgumentNullException.ThrowIfNull(convert);
        return result.IsSuccess
            ? Result<TTarget>.Ok(convert(result.Data))
            : Result<TTarget>.FromFailure(result);
    }
}
