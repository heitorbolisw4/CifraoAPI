using System.Text.Json.Serialization;

namespace BackEnd.DTO;

public class ExchangeRatesContracts
{
    public record ExchangeRatesResponse(string BaseCurrency, string QuoteCurrency, DateOnly RateDate ,decimal Bid, decimal Ask, string Source, DateTime FetchedAt);


}