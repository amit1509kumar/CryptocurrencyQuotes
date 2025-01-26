namespace CryptoCurrencyQuotesApi.Infrastructure.Models
{
    public class Quote
    {
        public required Currency EUR { get; set; }
    }

    public class Currency
    {
        public decimal Price { get; set; }
    }

    public class Crypto
    {
        public required Quote Quote { get; set; }
    }

    public class CoinMarketCapResponse
    {
        public required Dictionary<string, Crypto> Data { get; set; }
    }
}
