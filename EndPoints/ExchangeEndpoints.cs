using BackEnd.Domain.Interface;

namespace BackEnd.EndPoints;

public static class ExchangeEndpoints
{
    public static IEndpointRouteBuilder MapExchangeEndpoints( this IEndpointRouteBuilder app)
    {
        app.MapGet("/exchange", GetAwesome);
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




}