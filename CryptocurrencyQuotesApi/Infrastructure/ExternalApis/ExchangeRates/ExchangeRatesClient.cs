using CryptoCurrencyQuotesApi.Infrastructure.Models;

namespace CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates
{
    public class ExchangeRatesClient : IExchangeRatesClient
    {
        private readonly IExchangeRatesApi _exchangeRatesApi;
        private readonly IConfiguration _configuration;

        public ExchangeRatesClient(IExchangeRatesApi exchangeRatesApi, IConfiguration configuration)
        {
            _exchangeRatesApi = exchangeRatesApi;
            _configuration = configuration;
        }

        public async Task<Dictionary<string, decimal>> GetAsync()
        {
            string apiKey = _configuration["ApiKeys:ExchangeRates"]!;

            ExchangeRatesResponse response = await _exchangeRatesApi.GetLatestRatesAsync(apiKey);

            return response?.Rates ?? new Dictionary<string, decimal>();
        }
    }
}
