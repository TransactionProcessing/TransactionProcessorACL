using Microsoft.Extensions.Configuration;

namespace TransactionProcessorACL.Bootstrapper
{
    using BusinessLogic.RequestHandlers;
    using Lamar;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using Common;
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Lamar.ServiceRegistry" />
    [ExcludeFromCodeCoverage]
    public class MediatorRegistry : ServiceRegistry
    {
        public MediatorRegistry(IConfiguration configuration) {
            string eventStoreConnectionString = configuration.GetValue<string>("EventStoreSettings:ConnectionString");
            int requestAuditStreamLifetimeDays = configuration.GetValue<int?>("AuditSettings:RequestAuditStreamLifetimeDays") ?? 7;
            this.AddKurrentDBClient(eventStoreConnectionString);

            this.AddSingleton(new RequestAuditStreamOptions { RequestAuditStreamLifetimeDays = requestAuditStreamLifetimeDays });
            this.AddSingleton<IRequestAuditRecorder, LoggingRequestAuditRecorder.KurrentDbRequestAuditRecorder>();
            this.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditPipelineBehavior<,>));
            this.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(VersionCheckRequestHandler).Assembly));
        }
    }
}
