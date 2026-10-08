using BackEnd.Domain.Exceptions;

namespace BackEnd.Domain.Entities;

public class ExchangeRate
{
    private ExchangeRate(string baseCurrency, string quoteCurrency, DateOnly rateDate, decimal bid, decimal ask, string source, DateTime fetchedAt)
    {
        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
        RateDate = rateDate;
        Bid = bid;
        Ask = ask;
        Source = source;
        FetchedAt = fetchedAt;
    }

    public int Id { get; set; }
    public string BaseCurrency { get; private set; }
    public string QuoteCurrency { get; private set; }
    public DateOnly RateDate { get; private set; }
    public decimal Bid { get; private set; }
    public decimal Ask { get; private set; }
    public string Source { get; private set; }
    public DateTime FetchedAt { get; private set; }


    public static ExchangeRate Create(string baseCurrency, string quoteCurrency, DateOnly rateDate,
    decimal bid, decimal ask, string source, DateTime fetchedAt)
    {
        ValidCurrency(baseCurrency, quoteCurrency);
        ValidRateDate(rateDate);
        ValidBidAsk(bid, ask);
        ValidSource(source);
        ValidFetchedDate(fetchedAt);
        return new ExchangeRate(baseCurrency, quoteCurrency, rateDate, bid, ask, source,fetchedAt);
    }

    private static void ValidCurrency(string baseCurrency, string quoteCurrency)
    {
        if(string.IsNullOrWhiteSpace(baseCurrency) || string.IsNullOrWhiteSpace(quoteCurrency))
        {
            throw new DomainException("Invalid Pair");
        }
    }
    private static void ValidRateDate(DateOnly rateDate)
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        if( rateDate >= tomorrow)
        {
            throw new DomainException("Invalid Rate Date");
        }
    }
    private static void ValidBidAsk(decimal bid, decimal ask)
    {
        if( bid <= 0 || ask <= 0)
        {
            throw new DomainException("Bid and Ask must be positive values");
        }
    }
    private static void ValidSource(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new DomainException("Invalid Source");
        }
    }
    private static void ValidFetchedDate(DateTime fetchedAt)
    {
        var tomorrow = DateTime.Now.AddDays(1);
        if( fetchedAt >= tomorrow)
        {
            throw new DomainException("Invalid Fetched Date");
        }
    }
}