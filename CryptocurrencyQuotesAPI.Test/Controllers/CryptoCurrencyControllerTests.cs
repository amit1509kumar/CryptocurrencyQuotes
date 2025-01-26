using AutoFixture;
using CryptoCurrencyQuotesApi.Controllers;
using CryptoCurrencyQuotesApi.Services;
using CryptoCurrencyQuotesApi.Services.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CryptocurrencyQuotesAPI.Test.Controllers
{
    [TestClass]
    public class CryptoCurrencyControllerTests
    {
        private Mock<ICryptoCurrencyService> _mockCryptoCurrencyService = default!;
        private CryptoCurrencyController _controller = default!;
        private IFixture _fixture = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCryptoCurrencyService = new Mock<ICryptoCurrencyService>();
            _controller = new CryptoCurrencyController(_mockCryptoCurrencyService.Object);
            _fixture = new Fixture();
        }

        [TestMethod]
        public async Task GetQuotes_WhenSuccess_ReturnsOk()
        {
            // Arrange
            var currencyCode = "USD";
            var QuotesDto = _fixture.Create<QuotesDto>();
            QuotesDto.Outcome = CurrencyQuotesOutcome.Success;
            _mockCryptoCurrencyService
                .Setup(service => service.GetQuotesAsync(currencyCode))
                .ReturnsAsync(QuotesDto);

            // Act
            var result = await _controller.GetQuotes(currencyCode);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200);
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(QuotesDto.Currencies);
        }

        [TestMethod]
        public async Task GetQuotes_WhenInvalidCurrency_ReturnsBadRequest()
        {
            // Arrange
            var QuotesDto = new QuotesDto
            {
                Outcome = CurrencyQuotesOutcome.InvalidCurrency,
                Currencies = new List<CurrencyDto>()
            };
            _mockCryptoCurrencyService
                .Setup(service => service.GetQuotesAsync(It.IsAny<string>()))
                .ReturnsAsync(QuotesDto);

            // Act
            var result = await _controller.GetQuotes("InvalidCurrency");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be("The input provided is invalid.");
        }

        [TestMethod]
        public async Task GetQuotes_WhenNoDataForSymbol_ReturnsNotFound()
        {
            // Arrange
            var QuotesDto = new QuotesDto
            {
                Outcome = CurrencyQuotesOutcome.NoDataForSymbol,
                Currencies = new List<CurrencyDto>()
            };
            _mockCryptoCurrencyService
                .Setup(service => service.GetQuotesAsync(It.IsAny<string>()))
                .ReturnsAsync(QuotesDto);

            // Act
            var result = await _controller.GetQuotes("UnknownCurrency");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be(404);
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().Be("No data found for the specified currency symbol.");
        }

        [TestMethod]
        public async Task GetQuotes_WhenExchangeRatesUnavailable_ReturnsServiceUnavailable()
        {
            // Arrange
            var QuotesDto = new QuotesDto
            {
                Outcome = CurrencyQuotesOutcome.ExchangeRatesUnavailable,
                Currencies = new List<CurrencyDto>()
            };
            _mockCryptoCurrencyService
                .Setup(service => service.GetQuotesAsync(It.IsAny<string>()))
                .ReturnsAsync(QuotesDto);

            // Act
            var result = await _controller.GetQuotes("BTC");

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(503);
            var serviceUnavailableResult = result as ObjectResult;
            serviceUnavailableResult!.Value.Should().Be("Exchange rates are currently unavailable.");
        }

        [TestMethod]
        public async Task GetQuotes_WhenUnknownOutcome_ReturnsInternalServerError()
        {
            // Arrange
            var QuotesDto = new QuotesDto
            {
                Outcome = CurrencyQuotesOutcome.Undefined,
                Currencies = new List<CurrencyDto>()
            };
            _mockCryptoCurrencyService
                .Setup(service => service.GetQuotesAsync(It.IsAny<string>()))
                .ReturnsAsync(QuotesDto);

            // Act
            var result = await _controller.GetQuotes("UnknownCurrency");

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult!.Value.Should().Be("Internal Server Error.");
        }
    }
}
