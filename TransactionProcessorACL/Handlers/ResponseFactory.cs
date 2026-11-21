using System;
using TransactionProcessing.UnionResults;

namespace TransactionProcessorACL.Handlers;

internal static class ResponseFactory
{
    public static Result<TTarget> FromResult<TSource, TTarget>(Result<TSource> result, Func<TSource, TTarget> convert)
    {
        ArgumentNullException.ThrowIfNull(convert);
        return result.Value switch
        {
            Success<TSource> success => Results.Ok(convert(success.Value)),
            Error error => error,
            ErrorList errors => errors,
            _ => new Error(ErrorType.Critical, "An uninitialized result has no outcome.")
        };
    }
}
