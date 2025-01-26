using CryptoCurrencyQuotesApi;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace CryptocurrencyQuotesAPI.Test
{
    [TestClass]
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
        private readonly ExceptionHandlingMiddleware _middleware;

        public ExceptionHandlingMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Test exception");

            _mockNext.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            context.Response.ContentType.Should().Contain("application/json");
            context.Response.Body.Seek(0, SeekOrigin.Begin);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenNoExceptionThrown_PassesRequestToNextDelegate()
        {
            // Arrange
            var context = new DefaultHttpContext();

            _mockNext.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
