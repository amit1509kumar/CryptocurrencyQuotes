namespace CryptoCurrencyQuotesApi.Services.Models
{
    public class QuotesDto
    {
        public required CurrencyQuotesOutcome Outcome { get; set; }
        public required IEnumerable<CurrencyDto> Currencies { get; set; }
    }
}
