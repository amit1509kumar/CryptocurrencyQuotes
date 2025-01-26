namespace CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap
{
    public interface ICoinMarketCapClient
    {
        Task<(bool status, decimal price)> GetCryptoPriceAsync(string cryptoSymbol);
    }
}
