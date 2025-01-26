namespace CryptoCurrencyQuotesApi.Services.Models
{
    public class CurrencyDto
    {
        public required string Currency { get; set; }

        public required decimal Price { get; set; }
    }
}
