using AutoFixture;
using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap;
using CryptoCurrencyQuotesApi.Infrastructure.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CryptocurrencyQuotesAPI.Test.Infrastructure.ExternalApis.CoinMarketCap
{
    [TestClass]
    public class CoinMarketCapClientTests
    {
        private Mock<ICoinMarketCapApi> _mockCoinMarketCapApi = default!;
        private Mock<IConfiguration> _mockConfiguration = default!;
        private CoinMarketCapClient _coinMarketCapClient = default!;
        private IFixture _fixture = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCoinMarketCapApi = new Mock<ICoinMarketCapApi>();
            _mockConfiguration = new Mock<IConfiguration>();
            _coinMarketCapClient = new CoinMarketCapClient(_mockCoinMarketCapApi.Object, _mockConfiguration.Object);
            _fixture = new Fixture();
        }

        [TestMethod]
        public async Task GetCryptoPriceAsync_WhenResponseDataIsNull_ReturnsFalseAndZero()
        {
            // Arrange
            var cryptoSymbol = _fixture.Create<string>();
            _mockConfiguration.Setup(c => c["ApiKeys:CoinMarketCap"]).Returns("test-api-key");

            var mockResponse = new CoinMarketCapResponse
            {
                Data = null!
            };

            _mockCoinMarketCapApi
                .Setup(api => api.GetCryptoQuoteAsync(cryptoSymbol, "EUR", "test-api-key"))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _coinMarketCapClient.GetCryptoPriceAsync(cryptoSymbol);

            // Assert
            result.status.Should().BeFalse();
            result.price.Should().Be(0);
        }

        [TestMethod]
        public async Task GetCryptoPriceAsync_WhenResponseDataIsValid_ReturnsTrueAndPrice()
        {
            // Arrange
            var cryptoSymbol = "BTC";
            var expectedPrice = 12345.67m; // Example price
            _mockConfiguration.Setup(c => c["ApiKeys:CoinMarketCap"]).Returns("test-api-key");

            var mockResponse = new CoinMarketCapResponse
            {
                Data = new Dictionary<string, Crypto>
                {
                    { cryptoSymbol, new Crypto
                        {
                            Quote = new Quote
                            {
                                EUR = new Currency
                                {
                                    Price = expectedPrice
                                }
                            }
                        }
                    }
                }
            };

            _mockCoinMarketCapApi
                .Setup(api => api.GetCryptoQuoteAsync(cryptoSymbol, "EUR", "test-api-key"))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _coinMarketCapClient.GetCryptoPriceAsync(cryptoSymbol);

            // Assert
            result.status.Should().BeTrue();
            result.price.Should().Be(expectedPrice);
        }

        [TestMethod]
        public async Task GetCryptoPriceAsync_WhenApiKeyIsMissing_ReturnsFalseAndZero()
        {
            // Arrange
            var cryptoSymbol = _fixture.Create<string>();

            _mockConfiguration.Setup(c => c["ApiKeys:CoinMarketCap"]).Returns((string)null!);

            // Act
            var result = await _coinMarketCapClient.GetCryptoPriceAsync(cryptoSymbol);

            // Assert
            result.status.Should().BeFalse();
            result.price.Should().Be(0);
        }

        [TestMethod]
        public async Task GetCryptoPriceAsync_WhenApiCallFails_ReturnsFalseAndZero()
        {
            // Arrange
            var cryptoSymbol = _fixture.Create<string>();
            _mockConfiguration.Setup(c => c["ApiKeys:CoinMarketCap"]).Returns("test-api-key");

            _mockCoinMarketCapApi
                .Setup(api => api.GetCryptoQuoteAsync(cryptoSymbol, "EUR", "test-api-key"))
                .ReturnsAsync((CoinMarketCapResponse)null!); // Simulating an API failure

            // Act
            var result = await _coinMarketCapClient.GetCryptoPriceAsync(cryptoSymbol);

            // Assert
            result.status.Should().BeFalse();
            result.price.Should().Be(0);
        }
    }
}
