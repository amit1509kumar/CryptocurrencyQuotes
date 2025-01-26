using CryptoCurrencyQuotesApi;
using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.CoinMarketCap;
using CryptoCurrencyQuotesApi.Infrastructure.ExternalApis.ExchangeRates;
using CryptoCurrencyQuotesApi.Services;
using Polly;
using Polly.Extensions.Http;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add configuration for API endpoints and keys
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Define a retry policy with exponential backoff
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // Handles 5xx, 408, and network failures
    .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // Handles 429
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

// Register the Refit client for CoinMarketCap API with Polly retry policy
builder.Services.AddRefitClient<ICoinMarketCapApi>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(configuration["ApiEndpoints:CoinMarketCap"]!);
    })
    .AddPolicyHandler(retryPolicy);

// Register the Refit client for the Exchange Rate API
builder.Services.AddRefitClient<IExchangeRatesApi>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(configuration["ApiEndpoints:ExchangeRate"]!);
    })
    .AddPolicyHandler(retryPolicy);

// Register CoinMarketCapClient as a service
builder.Services.AddTransient<ICoinMarketCapClient, CoinMarketCapClient>();
builder.Services.AddTransient<IExchangeRatesClient, ExchangeRatesClient>();

builder.Services.AddScoped<ICryptoCurrencyService, CryptoCurrencyService>();
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CryptoCurrencyQuotes");
    });

    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
