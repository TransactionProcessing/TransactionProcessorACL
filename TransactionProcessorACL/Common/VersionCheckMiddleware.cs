using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;

namespace TransactionProcessorACL.Middleware;

public sealed class VersionCheckMiddleware {
    private readonly RequestDelegate _next;

    public VersionCheckMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,
                                  IMediator mediator) {
        var path = context.Request.Path;

        if (path.StartsWithSegments("/health") || path.StartsWithSegments("/healthui")) {
            await _next(context).ConfigureAwait(false);
            return;
        }

        context.Request.EnableBuffering();

        string? applicationVersion = await ExtractApplicationVersionAsync(context).ConfigureAwait(false);

        CancellationToken cancellationToken = context.RequestAborted;

        var versionCheckCommand = new VersionCheckCommands.VersionCheckCommand(applicationVersion);
        Result versionCheckResult = await mediator.Send(versionCheckCommand, cancellationToken).ConfigureAwait(false);

        if (versionCheckResult.IsFailed) {
            context.Response.StatusCode = 505;
            return; // stop the pipeline
        }

        await _next(context).ConfigureAwait(false);
    }

    private static async Task<string?> ExtractApplicationVersionAsync(HttpContext context) {
        // Only read the body if it's JSON (optional safety check)
        if (context.Request.ContentType?.Contains("application/json") == true) {
            using var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

            string body = await reader.ReadToEndAsync().ConfigureAwait(false);

            // Reset the request body stream position so the endpoint can read it
            context.Request.Body.Position = 0;

            try {
                using var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("application_version", out JsonElement versionProp)) {
                    return versionProp.GetString();
                }
            }
            catch {
                // Ignore JSON parse errors — allow request to continue
            }
        }

        // Fallback to querystring if needed
        var qs = context.Request.Query["applicationVersion"];
        return string.IsNullOrEmpty(qs) ? null : qs.ToString();
    }
}