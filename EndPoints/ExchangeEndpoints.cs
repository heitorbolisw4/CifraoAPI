using BackEnd.Data;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Interface;
using BackEnd.Service;
using Microsoft.EntityFrameworkCore;
using static BackEnd.DTO.ExchangeRatesContracts;

namespace BackEnd.EndPoints;

public static class ExchangeEndpoints
{
    public static IEndpointRouteBuilder MapExchangeEndpoints( this IEndpointRouteBuilder app)
    {
        app.MapGet("/exchange/today", GetAwesome);
        app.MapPost("/exchange/{pair}", SaveRate);
        return app;
    }

    private static async Task<IResult> GetAwesome(ExchangeRateAppService service, CancellationToken ct)
    {
        var pair = "USD-BRL";
        
        var rate = await service.GetOrFetchAsync(pair, ct);
        
        return rate is null ? Results.NotFound() : Results.Ok(ToResponse(rate));
        
    }
    private static async Task<IResult> SaveRate(string pair, ExchangeRateAppService service, CancellationToken ct)
    {
        var rate = await service.FetchAndSaveAsync(pair, ct);
        return rate is null ? Results.NotFound() : Results.Created($"/exchange/{rate.Id}", ToResponse(rate));
    }
    private static ExchangeRatesResponse ToResponse(ExchangeRate exchangeRate)
    {
        return new ExchangeRatesResponse(exchangeRate.BaseCurrency, exchangeRate.QuoteCurrency, exchangeRate.RateDate,
        exchangeRate.Bid, exchangeRate.Ask, exchangeRate.Source, exchangeRate.FetchedAt);
    }




}