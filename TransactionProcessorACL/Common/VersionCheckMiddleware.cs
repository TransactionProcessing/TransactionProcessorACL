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
                await _next(context);
                return;
            }

            context.Request.EnableBuffering();

            string? applicationVersion = null;

            if (context.Request.ContentType?.Contains("application/json") == true) {
                using var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

                string body = await reader.ReadToEndAsync().ConfigureAwait(false);

                // Reset the request body stream position so the endpoint can read it
                context.Request.Body.Position = 0;

                try {
                    using var json = JsonDocument.Parse(body);
                    if (json.RootElement.TryGetProperty("application_version", out JsonElement versionProp)) {
                        applicationVersion = versionProp.GetString();
                    }
                }
                catch {
                    // Ignore JSON parse errors — allow request to continue
                }
            }

            // Fallback to querystring if needed
            if (string.IsNullOrEmpty(applicationVersion)) {
                applicationVersion = context.Request.Query["applicationVersion"];
            }

            CancellationToken cancellationToken = context.RequestAborted;

            VersionCheckCommands.VersionCheckCommand versionCheckCommand = new(applicationVersion);
            Result versionCheckResult = await mediator.Send(versionCheckCommand, cancellationToken).ConfigureAwait(false);
            if (versionCheckResult.IsFailed) {
                context.Response.StatusCode = 505;
                return; // stop the pipeline
            }

            await _next(context).ConfigureAwait(false);
        }
    }