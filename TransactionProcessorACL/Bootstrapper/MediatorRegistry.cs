using Microsoft.Extensions.Configuration;
using SimpleResults;
using System.Collections.Generic;

namespace TransactionProcessorACL.Bootstrapper
{
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using Lamar;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using System.Diagnostics.CodeAnalysis;
    using TransactionProcessorACL.Common;
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Lamar.ServiceRegistry" />
    [ExcludeFromCodeCoverage]
    public class MediatorRegistry : ServiceRegistry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediatorRegistry"/> class.
        /// </summary>
        public MediatorRegistry()
        {
            string eventStoreConnectionString = Startup.Configuration.GetValue<string>("EventStoreSettings:ConnectionString");
            int requestAuditStreamLifetimeDays = Startup.Configuration.GetValue<int?>("AuditSettings:RequestAuditStreamLifetimeDays") ?? 7;
            this.AddKurrentDBClient(eventStoreConnectionString);

            this.AddSingleton(new RequestAuditStreamOptions
            {
                RequestAuditStreamLifetimeDays = requestAuditStreamLifetimeDays
            });
            this.AddSingleton<IRequestAuditRecorder, LoggingRequestAuditRecorder.KurrentDbRequestAuditRecorder>();
            this.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditPipelineBehavior<,>));
            this.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(VersionCheckRequestHandler).Assembly));
        }

        #endregion
    }
}
