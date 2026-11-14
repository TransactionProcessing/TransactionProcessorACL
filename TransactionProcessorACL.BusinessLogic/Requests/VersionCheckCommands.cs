using System;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using TransactionProcessing.UnionResults;

namespace TransactionProcessorACL.BusinessLogic.Requests;

[ExcludeFromCodeCoverage]
public record VersionCheckCommands {
    public record VersionCheckCommand(String VersionNumber) : IRequest<Result>;
}