namespace CryptoCurrencyQuotesApi.Infrastructure.Models
{
    public class ExchangeRatesResponse
    {
        public required string Base { get; set; }
        public required string Date { get; set; }
        public required Dictionary<string, decimal> Rates { get; set; }
    }
}
