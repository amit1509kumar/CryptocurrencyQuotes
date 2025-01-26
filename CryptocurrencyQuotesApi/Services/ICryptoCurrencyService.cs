using CryptoCurrencyQuotesApi.Services.Models;

namespace CryptoCurrencyQuotesApi.Services
{
    public interface ICryptoCurrencyService
    {
        Task<QuotesDto> GetQuotesAsync(string currencySymbol);
    }
}
