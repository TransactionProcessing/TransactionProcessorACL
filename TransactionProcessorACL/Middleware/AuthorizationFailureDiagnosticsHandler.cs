using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TransactionProcessorACL.Middleware;

/// <summary>
/// Temporary diagnostics for authorization failures. This logs policy metadata only.
/// </summary>
public sealed class AuthorizationFailureDiagnosticsHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler InnerHandler = new();
    private readonly ILogger<AuthorizationFailureDiagnosticsHandler> Logger;

    public AuthorizationFailureDiagnosticsHandler(ILogger<AuthorizationFailureDiagnosticsHandler> logger)
    {
        this.Logger = logger;
    }

    public async Task HandleAsync(RequestDelegate next,
                                   HttpContext context,
                                   AuthorizationPolicy policy,
                                   PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded == false)
        {
            string policyRequirements = String.Join(", ", policy.Requirements.Select(requirement => requirement.GetType().FullName));
            string failedRequirements = authorizeResult.AuthorizationFailure == null
                                            ? String.Empty
                                            : String.Join(", ", authorizeResult.AuthorizationFailure.FailedRequirements.Select(requirement => requirement.GetType().FullName));
            string failureReasons = authorizeResult.AuthorizationFailure == null
                                        ? String.Empty
                                        : String.Join("; ", authorizeResult.AuthorizationFailure.FailureReasons.Select(reason => reason.Message));

            this.Logger.LogWarning("Authorization failure diagnostics: Method={Method}; Path={Path}; Requirements={Requirements}; FailedRequirements={FailedRequirements}; FailureReasons={FailureReasons}",
                                   context.Request.Method,
                                   context.Request.Path,
                                   policyRequirements,
                                   failedRequirements,
                                   failureReasons);
        }

        await this.InnerHandler.HandleAsync(next, context, policy, authorizeResult).ConfigureAwait(false);
    }
}
