using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap;
using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates;
using CryptoCurrencyQuotesApi.Services.Models;

namespace CryptoCurrencyQuotesApi.Services
{
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private readonly ICoinMarketCapClient _coinMarketCapClient;
        private readonly IExchangeRatesClient _exchangeRatesClient;
        private readonly IConfiguration _configuration;

        public CryptoCurrencyService(ICoinMarketCapClient coinMarketCapClient, IExchangeRatesClient exchangeRatesClient,
                                     IConfiguration configuration)
        {
            _coinMarketCapClient = coinMarketCapClient;
            _exchangeRatesClient = exchangeRatesClient;
            _configuration = configuration;
        }

        public async Task<QuotesDto> GetQuotesAsync(string currencySymbol)
        {
            var currencies = new List<CurrencyDto>();

            //Invalid input
            if (string.IsNullOrEmpty(currencySymbol))
            {
                return new QuotesDto() { Outcome = CurrencyQuotesOutcome.InvalidCurrency, Currencies = currencies };
            }

            (bool status, decimal cryptoPrice) = await _coinMarketCapClient.GetCryptoPriceAsync(currencySymbol.ToUpper());

            //No data found for currency symbol
            if (!status)
            {
                return new QuotesDto() { Outcome = CurrencyQuotesOutcome.NoDataForSymbol, Currencies = currencies };
            }

            string supportedCurrencies = _configuration["SupportedCurrencies"]!;

            Dictionary<string, decimal> exchangeRates = await _exchangeRatesClient.GetAsync();

            //No data found for exchange rates
            if (exchangeRates.Count == 0)
            {
                return new QuotesDto() { Outcome = CurrencyQuotesOutcome.ExchangeRatesUnavailable, Currencies = currencies };
            }

            foreach (string supportedCurrency in supportedCurrencies.Split(","))
            {
                currencies.Add(new CurrencyDto() { Currency = supportedCurrency!, Price = Math.Round(exchangeRates[supportedCurrency] * cryptoPrice, 2) });
            }

            return new QuotesDto() { Outcome = CurrencyQuotesOutcome.Success, Currencies = currencies };
        }
    }
}
