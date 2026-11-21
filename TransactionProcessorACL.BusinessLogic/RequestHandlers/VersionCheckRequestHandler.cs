using TransactionProcessing.UnionResults;

namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Requests;
    using TransactionProcessing.Shared.General;

    public class VersionCheckRequestHandler : IRequestHandler<VersionCheckCommands.VersionCheckCommand, Result>
    {
        #region Methods

        public async Task<Result> Handle(VersionCheckCommands.VersionCheckCommand command,
                                         CancellationToken cancellationToken) {
            if (Boolean.TryParse(ConfigurationReader.GetValueOrDefault("AppSettings", "SkipVersionCheck", "false"), out Boolean skipVersionCheck) && skipVersionCheck) {
                return Results.Ok();
            }

            // Get the minimum version from the config
            String versionFromConfig = ConfigurationReader.GetValue("AppSettings", "MinimumSupportedApplicationVersion");

            // Convert to an assembly version
            Version minimumVersion = Version.Parse(versionFromConfig);

            Version.TryParse(command.VersionNumber, out Version requestVersion);
            
            Result result = requestVersion switch {
                null => new Error(ErrorType.Conflict, $"Version Mismatch - Version number was not provided"),
                _ when requestVersion.CompareTo(minimumVersion) < 0 => new Error(ErrorType.Conflict, $"Version Mismatch - Version number [{requestVersion}] is less than the Minimum Supported version [{minimumVersion}]"),
                _ => Results.Ok()
            };
            return result;
        }

        #endregion
    }
}