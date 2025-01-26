namespace CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates
{
    public interface IExchangeRatesClient
    {
        Task<Dictionary<string, decimal>> GetAsync();
    }
}
