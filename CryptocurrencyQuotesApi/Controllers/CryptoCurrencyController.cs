using CryptoCurrencyQuotesApi.Services;
using CryptoCurrencyQuotesApi.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptoCurrencyQuotesApi.Controllers
{
    [RequireHttps]
    [ApiController]
    [Route("api/v1/cryptocurrency")]
    [Produces("application/json")]
    public class CryptoCurrencyController : ControllerBase
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public CryptoCurrencyController(ICryptoCurrencyService cryptoCurrencyService)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
        }

        /// <summary>
        /// Gets the quotes for a specific cryptocurrency.
        /// </summary>
        [HttpGet]
        [Route("quotes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuotes(string currencyCode)
        {
            QuotesDto quote = await _cryptoCurrencyService.GetQuotesAsync(currencyCode);

            return quote.Outcome switch
            {
                CurrencyQuotesOutcome.Success => Ok(quote.Currencies),
                CurrencyQuotesOutcome.InvalidCurrency => BadRequest("The input provided is invalid."),
                CurrencyQuotesOutcome.NoDataForSymbol => NotFound("No data found for the specified currency symbol."),
                CurrencyQuotesOutcome.ExchangeRatesUnavailable => StatusCode(503, "Exchange rates are currently unavailable."),
                _ => StatusCode(500, "Internal Server Error.")
            };
        }
    }
}
