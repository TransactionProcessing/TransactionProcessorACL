using Imposter.Abstractions;
using Microsoft.AspNetCore.Hosting;
using SecurityService.Client;
using TransactionProcessor.Client;
using TransactionProcessorACL.BusinessLogic.BackendAPI;
using TransactionProcessorACL.BusinessLogic.Services;

[assembly: GenerateImposter(typeof(ITransactionProcessorClient))]
[assembly: GenerateImposter(typeof(ISecurityServiceClient))]
[assembly: GenerateImposter(typeof(IEstateReportingApiClient))]
[assembly: GenerateImposter(typeof(ITransactionProcessorACLApplicationService))]
[assembly: GenerateImposter(typeof(IWebHostEnvironment))]
