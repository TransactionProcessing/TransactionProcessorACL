using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Imposter.Abstractions;
using Shouldly;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Middleware;
using Xunit;

namespace TransactionProcessorACL.Tests.General
{
    public class VersionCheckMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenMediatorReturnsSuccess_CallsNextAndLeavesStatusUnchanged()
        {
            // Arrange
            var json = "{\"application_version\":\"1.2.3\"}";
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/transactions";
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            context.Request.ContentLength = context.Request.Body.Length;

            bool nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };

            var mediatorMock = new IMediatorImposter(ImposterMode.Explicit);
            mediatorMock
                .Send<Result>(Arg<MediatR.IRequest<Result>>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success());

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Instance());

            // Assert
            nextCalled.ShouldBeTrue();
            context.Response.StatusCode.ShouldBe(200);
            mediatorMock.Send<Result>(Arg<MediatR.IRequest<Result>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
        }

        [Fact]
        public async Task InvokeAsync_WhenMediatorReturnsFailure_SetsStatus505AndDoesNotCallNext()
        {
            // Arrange
            var json = "{\"application_version\":\"1.2.3\"}";
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/transactions";
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            context.Request.ContentLength = context.Request.Body.Length;

            bool nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };

            var mediatorMock = new IMediatorImposter(ImposterMode.Explicit);
            mediatorMock
                .Send<Result>(Arg<MediatR.IRequest<Result>>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure()); // simulate old/invalid version

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Instance());

            // Assert
            nextCalled.ShouldBeFalse();
            context.Response.StatusCode.ShouldBe(505);
            mediatorMock.Send<Result>(Arg<MediatR.IRequest<Result>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
        }

        [Fact]
        public async Task InvokeAsync_WhenPathIsHealth_SkipsVersionCheckAndCallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/health/ready";

            bool nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };

            var mediatorMock = new IMediatorImposter(ImposterMode.Explicit);
            // mediator should not be called for health paths

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Instance());

            // Assert
            nextCalled.ShouldBeTrue();
            context.Response.StatusCode.ShouldBe(200);
            mediatorMock.Send<Result>(Arg<MediatR.IRequest<Result>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Never());
        }

        [Fact]
        public async Task InvokeAsync_WhenNoBody_AndVersionInQueryString_UsesQueryValue()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/transactions";
            context.Request.QueryString = new QueryString("?applicationVersion=9.9.9");

            bool nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };

            var mediatorMock = new IMediatorImposter(ImposterMode.Explicit);
            mediatorMock
                .Send<Result>(Arg<MediatR.IRequest<Result>>.Is(request => request is VersionCheckCommands.VersionCheckCommand command && command.VersionNumber == "9.9.9"), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success());

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Instance());

            // Assert
            nextCalled.ShouldBeTrue();
            mediatorMock.Send<Result>(Arg<MediatR.IRequest<Result>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
        }
    }
}
