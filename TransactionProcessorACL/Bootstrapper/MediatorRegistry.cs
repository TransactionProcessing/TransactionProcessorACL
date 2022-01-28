﻿namespace TransactionProcessorACL.Bootstrapper
{
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using Lamar;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Lamar.ServiceRegistry" />
    public class MediatorRegistry : ServiceRegistry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediatorRegistry"/> class.
        /// </summary>
        public MediatorRegistry()
        {
            this.AddTransient<IMediator, Mediator>();
            this.AddTransient<ServiceFactory>(context => { return t => context.GetService(t); });
            this.AddSingleton<IRequestHandler<VersionCheckRequest, Unit>, VersionCheckRequestHandler>();
            this.AddSingleton<IRequestHandler<ProcessLogonTransactionRequest, ProcessLogonTransactionResponse>, ProcessLogonTransactionRequestHandler>();
            this.AddSingleton<IRequestHandler<ProcessSaleTransactionRequest, ProcessSaleTransactionResponse>, ProcessSaleTransactionRequestHandler>();
            this.AddSingleton<IRequestHandler<ProcessReconciliationRequest, ProcessReconciliationResponse>, ProcessReconciliationRequestHandler>();
            this.AddSingleton<IRequest<ProcessLogonTransactionResponse>, ProcessLogonTransactionRequest>();
            this.AddSingleton<IRequest<ProcessSaleTransactionResponse>, ProcessSaleTransactionRequest>();
            this.AddSingleton<IRequest<ProcessReconciliationResponse>, ProcessReconciliationRequest>();
        }

        #endregion
    }
}