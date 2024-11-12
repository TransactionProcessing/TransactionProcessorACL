using SimpleResults;

namespace TransactionProcessorACL.Bootstrapper
{
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using Lamar;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using System.Diagnostics.CodeAnalysis;
    
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
            this.AddTransient<IMediator, Mediator>();
            
            this.AddSingleton<IRequestHandler<VersionCheckCommands.VersionCheckCommand, Result>, VersionCheckRequestHandler>();
            
            this.AddSingleton<IRequestHandler<TransactionCommands.ProcessLogonTransactionCommand, Result<ProcessLogonTransactionResponse>>, TransactionRequestHandler>();
            this.AddSingleton<IRequestHandler<TransactionCommands.ProcessSaleTransactionCommand, Result<ProcessSaleTransactionResponse>>, TransactionRequestHandler>();
            this.AddSingleton<IRequestHandler<TransactionCommands.ProcessReconciliationCommand, Result<ProcessReconciliationResponse>>, TransactionRequestHandler>();
            
            this.AddSingleton<IRequestHandler<VoucherQueries.GetVoucherQuery, Result<GetVoucherResponse>>, VoucherRequestHandler>();
            this.AddSingleton<IRequestHandler<VoucherCommands.RedeemVoucherCommand, Result<RedeemVoucherResponse>>, VoucherRequestHandler>();
        }

        #endregion
    }
}