using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates;
using CryptoCurrencyQuotesApi.Infrastructure.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CryptocurrencyQuotesAPI.Test.Infrastructure.ExternalApis.ExchangeRates
{
    [TestClass]
    public class ExchangeRatesClientTests
    {
        private Mock<IExchangeRatesApi> _mockExchangeRatesApi = default!;
        private Mock<IConfiguration> _mockConfiguration = default!;
        private ExchangeRatesClient _exchangeRatesClient = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockExchangeRatesApi = new Mock<IExchangeRatesApi>();
            _mockConfiguration = new Mock<IConfiguration>();
            _exchangeRatesClient = new ExchangeRatesClient(_mockExchangeRatesApi.Object, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task GetAsync_WhenResponseRatesIsValid_ReturnsRates()
        {
            // Arrange
            var expectedRates = new Dictionary<string, decimal>
            {
                { "USD", 1.12m },
                { "EUR", 0.93m },
                { "GBP", 0.85m }
            };

            var mockResponse = new ExchangeRatesResponse
            {
                Base = "EUR",  // Base currency
                Date = "2025-01-01",  // Example date
                Rates = expectedRates
            };

            _mockConfiguration.Setup(c => c["ApiKeys:ExchangeRates"]).Returns("test-api-key");

            _mockExchangeRatesApi
                .Setup(api => api.GetLatestRatesAsync("test-api-key"))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _exchangeRatesClient.GetAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedRates);
        }

        [TestMethod]
        public async Task GetAsync_WhenResponseRatesIsNull_ReturnsEmptyDictionary()
        {
            // Arrange
            var mockResponse = new ExchangeRatesResponse
            {
                Base = "EUR",
                Date = "2025-01-01",
                Rates = null!
            };

            _mockConfiguration.Setup(c => c["ApiKeys:ExchangeRates"]).Returns("test-api-key");

            _mockExchangeRatesApi
                .Setup(api => api.GetLatestRatesAsync("test-api-key"))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _exchangeRatesClient.GetAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAsync_WhenApiKeyIsMissing_ReturnsEmptyDictionary()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ApiKeys:ExchangeRates"]).Returns((string)null!);

            // Act
            var result = await _exchangeRatesClient.GetAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAsync_WhenApiCallFails_ReturnsEmptyDictionary()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ApiKeys:ExchangeRates"]).Returns("test-api-key");

            _mockExchangeRatesApi
                .Setup(api => api.GetLatestRatesAsync("test-api-key"))
                .ReturnsAsync((ExchangeRatesResponse)null!); // Simulating an API failure

            // Act
            var result = await _exchangeRatesClient.GetAsync();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
