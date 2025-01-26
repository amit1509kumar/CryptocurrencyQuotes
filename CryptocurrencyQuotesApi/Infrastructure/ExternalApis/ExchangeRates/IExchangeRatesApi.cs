using CryptoCurrencyQuotesApi.Infrastructure.Models;
using Refit;

namespace CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates
{
    public interface IExchangeRatesApi
    {
        [Get("/v1/latest")]
        Task<ExchangeRatesResponse> GetLatestRatesAsync([AliasAs("access_key")] string accessKey);
    }
}
