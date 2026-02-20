using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
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

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            mediatorMock
                .Setup(m => m.Send(It.IsAny<VersionCheckCommands.VersionCheckCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success);

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Object);

            // Assert
            nextCalled.ShouldBeTrue();
            context.Response.StatusCode.ShouldBe(200);
            mediatorMock.Verify(m => m.Send(It.IsAny<VersionCheckCommands.VersionCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            mediatorMock
                .Setup(m => m.Send(It.IsAny<VersionCheckCommands.VersionCheckCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure); // simulate old/invalid version

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Object);

            // Assert
            nextCalled.ShouldBeFalse();
            context.Response.StatusCode.ShouldBe(505);
            mediatorMock.Verify(m => m.Send(It.IsAny<VersionCheckCommands.VersionCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            // mediator should not be called for health paths

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Object);

            // Assert
            nextCalled.ShouldBeTrue();
            context.Response.StatusCode.ShouldBe(200);
            mediatorMock.Verify(m => m.Send(It.IsAny<VersionCheckCommands.VersionCheckCommand>(), It.IsAny<CancellationToken>()), Times.Never);
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

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            mediatorMock
                .Setup(m => m.Send(It.Is<VersionCheckCommands.VersionCheckCommand>(cmd => cmd.VersionNumber == "9.9.9"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success);

            var middleware = new VersionCheckMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, mediatorMock.Object);

            // Assert
            nextCalled.ShouldBeTrue();
            mediatorMock.Verify(m => m.Send(It.IsAny<VersionCheckCommands.VersionCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}