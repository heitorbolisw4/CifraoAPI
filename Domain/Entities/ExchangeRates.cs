namespace BackEnd.Domain.Entities;

public class ExchangeRates
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public DateOnly RateDate { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime FetchedAt { get; set; }
}