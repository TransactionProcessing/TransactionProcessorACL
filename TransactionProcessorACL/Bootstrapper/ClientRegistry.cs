namespace TransactionProcessorACL.Bootstrapper
{
    using ClientProxyBase;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using SecurityService.Client;
    using Shared.General;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using TransactionProcessor.Client;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Lamar.ServiceRegistry" />
    [ExcludeFromCodeCoverage]
    public class ClientRegistry : ServiceRegistry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRegistry"/> class.
        /// </summary>
        public ClientRegistry() {
            this.AddHttpContextAccessor();
            this.RegisterHttpClient<ISecurityServiceClient, SecurityServiceClient>();
            this.RegisterHttpClient<ITransactionProcessorClient, TransactionProcessorClient>();
            Func<String, String> resolver(IServiceProvider container) => serviceName => ConfigurationReader.GetBaseServerUri(serviceName).OriginalString;
            this.AddSingleton<Func<String, String>>(resolver);
        }

        #endregion
    }
}