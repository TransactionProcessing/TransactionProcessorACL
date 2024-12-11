using SimpleResults;

namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Newtonsoft.Json;
    using Requests;
    using Shared.General;
    using Shared.Logger;
    using Shared.Middleware;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VersionCheckRequest" />
    public class VersionCheckRequestHandler : IRequestHandler<VersionCheckCommands.VersionCheckCommand, Result>
    {
        #region Methods

        public async Task<Result> Handle(VersionCheckCommands.VersionCheckCommand command,
                                         CancellationToken cancellationToken) {
            if (Boolean.TryParse(ConfigurationReader.GetValueOrDefault("AppSettings", "SkipVersionCheck", "false"), out Boolean skipVersionCheck) && skipVersionCheck) {
                return Result.Success();
            }

            // Get the minimum version from the config
            String versionFromConfig = ConfigurationReader.GetValue("AppSettings", "MinimumSupportedApplicationVersion");

            // Convert to an assembly version
            Version minimumVersion = Version.Parse(versionFromConfig);

            Version.TryParse(command.VersionNumber, out Version requestVersion);
            
            Result result = requestVersion switch {
                null => Result.Conflict($"Version Mismatch - Version number was not provided"),
                _ when requestVersion.CompareTo(minimumVersion) < 0 => Result.Conflict($"Version Mismatch - Version number [{requestVersion}] is less than the Minimum Supported version [{minimumVersion}]"),
                _ => Result.Success()
            };
            return result;
        }

        #endregion
    }
}