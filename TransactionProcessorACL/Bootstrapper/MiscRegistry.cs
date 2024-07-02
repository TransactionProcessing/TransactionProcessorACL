using Microsoft.Extensions.Logging;

namespace TransactionProcessorACL.Bootstrapper
{
    using Factories;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.General;
    using Shared.Middleware;
    using System.Collections.Generic;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Lamar.ServiceRegistry" />
    [ExcludeFromCodeCoverage]
    public class MiscRegistry : ServiceRegistry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MiscRegistry"/> class.
        /// </summary>
        public MiscRegistry()
        {
            bool logRequests = ConfigurationReaderExtensions.GetValueOrDefault<Boolean>("MiddlewareLogging", "LogRequests", true);
            bool logResponses = ConfigurationReaderExtensions.GetValueOrDefault<Boolean>("MiddlewareLogging", "LogResponses", true);
            LogLevel middlewareLogLevel = ConfigurationReaderExtensions.GetValueOrDefault<LogLevel>("MiddlewareLogging", "MiddlewareLogLevel", LogLevel.Warning);

            RequestResponseMiddlewareLoggingConfig config =
                new RequestResponseMiddlewareLoggingConfig(middlewareLogLevel, logRequests, logResponses);

            this.AddSingleton<IModelFactory, ModelFactory>();

            this.AddSingleton(config);
        }

        #endregion
    }

    public static class ConfigurationReaderExtensions
    {
        public static T GetValueOrDefault<T>(String sectionName, String keyName, T defaultValue)
        {
            try
            {
                var value = ConfigurationReader.GetValue(sectionName, keyName);

                if (String.IsNullOrEmpty(value))
                {
                    return defaultValue;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (KeyNotFoundException kex)
            {
                return defaultValue;
            }
        }
    }
}