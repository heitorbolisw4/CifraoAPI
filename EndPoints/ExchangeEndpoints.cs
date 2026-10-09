using BackEnd.Domain.Interface;
using BackEnd.Service;

namespace BackEnd.EndPoints;

public static class ExchangeEndpoints
{
    public static IEndpointRouteBuilder MapExchangeEndpoints( this IEndpointRouteBuilder app)
    {
        app.MapGet("/exchange", GetAwesome);
        app.MapPost("/exchange/{pair}", SaveRate);
        return app;
    }

    private static async Task<IResult> GetAwesome(IExchangeRateService rateService)
    {
        var pair = "USD-BRL";
        var response = await rateService.GetQuoteResponseAsync(pair);
        return  response is null
        ?   Results.NotFound()
        :   Results.Ok(response);
    }
    private static async Task<IResult> SaveRate(string pair, ExchangeRateAppService service, CancellationToken ct)
    {
        var rate = await service.FetchAndSaveAsync(pair, ct);
        return rate is null ? Results.NotFound() : Results.Created($"/exchange/{rate.Id}", rate);
    }




}