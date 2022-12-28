namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
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
    public class VersionCheckRequestHandler : IRequestHandler<VersionCheckRequest>
    {
        #region Methods

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Version number [{requestVersion}] is less than the Minimum Supported version [{minimumVersion}]</exception>
        public async Task<Unit> Handle(VersionCheckRequest request,
                                       CancellationToken cancellationToken) {
            if (Environment.GetEnvironmentVariable("AppSettings:SkipVersionCheck") != null) {
                if (Boolean.TryParse(ConfigurationReader.GetValue("AppSettings","SkipVersionCheck"), out Boolean skipVersionCheck) && skipVersionCheck) {
                    return default;
                }
            }

            // Get the minimum version from the config
            String versionFromConfig = ConfigurationReader.GetValue("AppSettings", "MinimumSupportedApplicationVersion");

            // Convert to an assembly version
            Version minimumVersion = Version.Parse(versionFromConfig);

            Version.TryParse(request.VersionNumber, out Version requestVersion);

            if (requestVersion == null || requestVersion.CompareTo(minimumVersion) < 0)
            {
                // This is not compatible
                throw new VersionIncompatibleException($"Version Mistmatch - Version number [{requestVersion}] is less than the Minimum Supported version [{minimumVersion}]");
            }

            return default;
        }

        #endregion
    }

    public class VersionIncompatibleException : Exception
    {
        public VersionIncompatibleException(String message) : base(message)
        {
            
        }

        public VersionIncompatibleException(String message, Exception innerException) : base(message, innerException)
        {

        }
    }
}