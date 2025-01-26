using CryptoCurrencyQuotesApi.Infrastructure.Models;
using Refit;

namespace CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap
{
    public interface ICoinMarketCapApi
    {
        [Get("/v1/cryptocurrency/quotes/latest")]
        Task<CoinMarketCapResponse> GetCryptoQuoteAsync(
        [Query] string symbol, [Query] string convert,
        [Header("X-CMC_PRO_API_KEY")] string apiKey);
    }
}
