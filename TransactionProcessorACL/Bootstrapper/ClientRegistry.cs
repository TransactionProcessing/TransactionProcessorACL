namespace TransactionProcessorACL.Bootstrapper
{
    using ClientProxyBase;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using SecurityService.Client;
    using Shared.General;
    using Shared.Serialisation;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using TransactionProcessorACL.Common;
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
        public ClientRegistry()
        {
            this.AddHttpContextAccessor();
            this.AddSingleton<RequestAuditContextAccessor>();
            this.AddTransient<CorrelationHeaderDelegatingHandler>();
            this.RegisterHttpClient<ISecurityServiceClient, SecurityServiceClient>()
                .AddHttpMessageHandler<CorrelationHeaderDelegatingHandler>();
            this.RegisterHttpClient<ITransactionProcessorClient, TransactionProcessorClient>()
                .AddHttpMessageHandler<CorrelationHeaderDelegatingHandler>();
            Func<String, String> resolver(IServiceProvider container) => serviceName => ConfigurationReader.GetBaseServerUri(serviceName).OriginalString;
            this.AddSingleton<Func<String, String>>(resolver);
        }

        #endregion
    }

    [ExcludeFromCodeCoverage]
    public class SerialiserRegistry : ServiceRegistry
    {
        public SerialiserRegistry()
        {
            this.AddSingleton<IStringSerialiser, SystemTextJsonSerializer>();
            this.AddSingleton<Func<Object, String>>(_ => obj => StringSerialiser.Serialise(obj));
            this.AddSingleton<Func<String, Type, Object>>(_ => (str, type) => StringSerialiser.DeserializeObject<Object>(str, type));

            var serialiserSettings = SystemTextJsonSerializer.GetDefaultJsonSerializerOptions();

            this.AddSingleton(serialiserSettings);
        }
    }
}
