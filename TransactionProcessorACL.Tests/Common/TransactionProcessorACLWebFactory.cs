﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.Tests.Common
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Requests;
    using MediatR;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public class TransactionProcessorACLWebFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Setup my mocks in here
            Mock<IMediator> mediatorMock = this.CreateMediatorMock();

            builder.ConfigureServices((builderContext, services) =>
            {
                if (mediatorMock != null)
                {
                    services.AddSingleton<IMediator>(mediatorMock.Object);
                }

                services.AddMvcCore(options =>
                {
                    options.Filters.Add(new AllowAnonymousFilter());

                })
                        .AddApplicationPart(typeof(Startup).Assembly);
            });
            ;
        }

        private Mock<IMediator> CreateMediatorMock()
        {
            Mock<IMediator> mediatorMock = new Mock<IMediator>(MockBehavior.Strict);

            mediatorMock.Setup(c => c.Send(It.IsAny<TransactionCommands.ProcessLogonTransactionCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProcessLogonTransactionResponse
                                                                                                                                    {
                ResponseCode = "0000",
                ResponseMessage = "SUCCESS"
                                                                                                                                    });

            return mediatorMock;
        }

    }

    /// <summary>
    /// </summary>
    /// <seealso cref="Startup" />
    [CollectionDefinition("TestCollection")]
    public class TestCollection : ICollectionFixture<TransactionProcessorACLWebFactory<Startup>>
    {
        // A class with no code, only used to define the collection
    }

    public static class Helpers
    {
        #region Methods

        /// <summary>
        /// Creates the content of the string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestObject">The request object.</param>
        /// <returns></returns>
        public static StringContent CreateStringContent<T>(T requestObject)
        {
            return new StringContent(JsonConvert.SerializeObject(requestObject, new JsonSerializerSettings
                                                                                {
                                                                                    TypeNameHandling = TypeNameHandling.All
                                                                                }), Encoding.UTF8, "application/json");
        }

        #endregion
    }

    public static class ServiceCollectionExtensions
    {
        public static void AssertConfigurationIsValid(this IServiceCollection serviceCollection,
                                                      List<Type> typesToIgnore = null)
        {
            ServiceProvider buildServiceProvider = serviceCollection.BuildServiceProvider();

            List<ServiceDescriptor> list = serviceCollection.Where(x => x.ServiceType.Namespace != null && x.ServiceType.Namespace.Contains("Vme")).ToList();

            if (typesToIgnore != null)
            {
                list.RemoveAll(listItem => typesToIgnore.Contains(listItem.ServiceType));
            }

            foreach (ServiceDescriptor serviceDescriptor in list)
            {
                Type type = serviceDescriptor.ServiceType;

                //This throws an Exception if the type cannot be instantiated.
                buildServiceProvider.GetService(type);
            }
        }
    }
}
