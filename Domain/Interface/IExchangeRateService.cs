using BackEnd.DTO;
using static BackEnd.DTO.AwesomeApiContracts;

namespace BackEnd.Domain.Interface;


public interface IExchangeRateService
{
    Task<QuoteResponse?> GetQuoteResponseAsync(string pair);
}