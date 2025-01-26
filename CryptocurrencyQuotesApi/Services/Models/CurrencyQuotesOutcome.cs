namespace CryptoCurrencyQuotesApi.Services.Models
{
    public enum CurrencyQuotesOutcome
    {
        Undefined = 0,
        Success,
        InvalidCurrency,
        NoDataForSymbol,
        ExchangeRatesUnavailable
    }
}
