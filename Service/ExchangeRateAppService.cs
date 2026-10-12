using System.Globalization;
using BackEnd.Data;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Service;


public class ExchangeRateAppService(AppDbContext db, IExchangeRateService service, ILogger<ExchangeRateAppService> logger)
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
    public async Task<ExchangeRate?> GetOrFetchAsync(string pair, CancellationToken ct)
    {
        var limite = DateTime.UtcNow.AddSeconds(-1);
        var parts = pair.Split('-');
        var baseCur = parts[0];
        var quoteCur = parts[1];

        var cached = await db.ExchangeRates.Where(x => x.BaseCurrency == baseCur 
        && x.QuoteCurrency == quoteCur
        && x.FetchedAt >= limite).OrderByDescending( x => x.FetchedAt ).FirstOrDefaultAsync(ct);
        
        if ( cached is not null )
            return cached;

        try
        {
            return await FetchAndSaveAsync(pair, ct);
            
        }
        catch (Exception ex) when ( ex is HttpRequestException or TaskCanceledException)
        {
            var lastRate = await db.ExchangeRates.Where(x => x.BaseCurrency == baseCur
            && x.QuoteCurrency == quoteCur)
            .OrderByDescending(x => x.FetchedAt).FirstOrDefaultAsync(ct);
            if(lastRate is null ) throw;
            logger.LogWarning(ex, "AwesomeAPI falhou. Usando cotação salva de {FetchedAt}", lastRate.FetchedAt);
            return lastRate;
        }
    }
    
}