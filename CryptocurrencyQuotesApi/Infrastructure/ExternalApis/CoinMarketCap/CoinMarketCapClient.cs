using CryptoCurrencyQuotesApi.Infrastructure.Models;

namespace CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap
{
    public class CoinMarketCapClient : ICoinMarketCapClient
    {
        private readonly ICoinMarketCapApi _coinMarketCapApi;
        private readonly IConfiguration _configuration;

        public CoinMarketCapClient(ICoinMarketCapApi coinMarketCapApi, IConfiguration configuration)
        {
            _coinMarketCapApi = coinMarketCapApi;
            _configuration = configuration;
        }

        public async Task<(bool status, decimal price)> GetCryptoPriceAsync(string cryptoSymbol)
        {
            string apiKey = _configuration["ApiKeys:CoinMarketCap"]!;

            CoinMarketCapResponse response = await _coinMarketCapApi.GetCryptoQuoteAsync(cryptoSymbol, "EUR", apiKey);

            if (response?.Data == null || !response.Data.ContainsKey(cryptoSymbol))
            {
                return (false, 0);
            }

            return (true, response.Data[cryptoSymbol].Quote.EUR.Price ?? 0);
        }
    }
}
