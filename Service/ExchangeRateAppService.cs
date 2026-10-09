using System.Globalization;
using BackEnd.Data;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Interface;

namespace BackEnd.Service;


public class ExchangeRateAppService(AppDbContext db, IExchangeRateService service)
{
    public async Task<ExchangeRate?> FetchAndSaveAsync(string pair, CancellationToken ct)
    {
        var dto = await service.GetQuoteResponseAsync(pair);
        if (dto is null) return null;



        var rate = ExchangeRate.Create( 
            dto.Code, dto.CodeIn,
            DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(dto.Timestamp)).UtcDateTime),
                decimal.Parse(dto.Bid, CultureInfo.InvariantCulture), 
                decimal.Parse(dto.Ask, CultureInfo.InvariantCulture), 
                "AwesomeAPI", DateTime.UtcNow);

        db.ExchangeRates.Add(rate);

        await db.SaveChangesAsync(ct);
        return rate;
    
    
    }
    
}