using System;
using MediatR;
using SimpleResults;

namespace TransactionProcessorACL.BusinessLogic.Requests;

public record VersionCheckCommands {
    public record VersionCheckCommand(String VersionNumber) : IRequest<Result>;
}