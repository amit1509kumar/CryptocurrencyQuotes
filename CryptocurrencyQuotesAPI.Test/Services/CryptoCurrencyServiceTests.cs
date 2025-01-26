using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap;
using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates;
using CryptoCurrencyQuotesApi.Services;
using CryptoCurrencyQuotesApi.Services.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CryptocurrencyQuotesAPI.Test.Services
{
    [TestClass]
    public class CryptoCurrencyServiceTests
    {
        private Mock<ICoinMarketCapClient> _mockCoinMarketCapClient = default!;
        private Mock<IExchangeRatesClient> _mockExchangeRatesClient = default!;
        private Mock<IConfiguration> _mockConfiguration = default!;
        private CryptoCurrencyService _cryptoCurrencyService = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCoinMarketCapClient = new Mock<ICoinMarketCapClient>();
            _mockExchangeRatesClient = new Mock<IExchangeRatesClient>();
            _mockConfiguration = new Mock<IConfiguration>();
            _cryptoCurrencyService = new CryptoCurrencyService(_mockCoinMarketCapClient.Object, _mockExchangeRatesClient.Object, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task GetQuotesAsync_WhenCurrencySymbolIsNull_ReturnsInvalidCurrencyOutcome()
        {
            // Act
            var result = await _cryptoCurrencyService.GetQuotesAsync(null!);

            // Assert
            result.Outcome.Should().Be(CurrencyQuotesOutcome.InvalidCurrency);
            result.Currencies.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetQuotesAsync_WhenNoDataForCurrency_ReturnsNoDataForSymbolOutcome()
        {
            // Arrange
            var currencySymbol = "BTC";
            _mockCoinMarketCapClient.Setup(client => client.GetCryptoPriceAsync(currencySymbol))
                                    .ReturnsAsync((false, 0));

            // Act
            var result = await _cryptoCurrencyService.GetQuotesAsync(currencySymbol);

            // Assert
            result.Outcome.Should().Be(CurrencyQuotesOutcome.NoDataForSymbol);
            result.Currencies.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetQuotesAsync_WhenExchangeRatesUnavailable_ReturnsExchangeRatesUnavailableOutcome()
        {
            // Arrange
            var currencySymbol = "BTC";
            _mockCoinMarketCapClient.Setup(client => client.GetCryptoPriceAsync(currencySymbol))
                                    .ReturnsAsync((true, 50000m));

            _mockExchangeRatesClient.Setup(client => client.GetAsync())
                                     .ReturnsAsync(new Dictionary<string, decimal>());

            // Act
            var result = await _cryptoCurrencyService.GetQuotesAsync(currencySymbol);

            // Assert
            result.Outcome.Should().Be(CurrencyQuotesOutcome.ExchangeRatesUnavailable);
            result.Currencies.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetQuotesAsync_WhenSuccessful_ReturnsQuoteWithCurrencies()
        {
            // Arrange
            var currencySymbol = "BTC";
            var supportedCurrencies = "USD,EUR,GBP";

            _mockCoinMarketCapClient.Setup(client => client.GetCryptoPriceAsync(currencySymbol))
                                    .ReturnsAsync((true, 50000m));

            var exchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 1.1m },
                { "EUR", 0.9m },
                { "GBP", 0.8m }
            };

            _mockExchangeRatesClient.Setup(client => client.GetAsync())
                                     .ReturnsAsync(exchangeRates);

            _mockConfiguration.Setup(config => config["SupportedCurrencies"]).Returns(supportedCurrencies);

            // Act
            var result = await _cryptoCurrencyService.GetQuotesAsync(currencySymbol);

            // Assert
            result.Outcome.Should().Be(CurrencyQuotesOutcome.Success);
            result.Currencies.Should().HaveCount(3);
            result.Currencies.Should().Contain(c => c.Currency == "USD" && c.Price == 55000);
            result.Currencies.Should().Contain(c => c.Currency == "EUR" && c.Price == 45000);
            result.Currencies.Should().Contain(c => c.Currency == "GBP" && c.Price == 40000);
        }
    }
}
