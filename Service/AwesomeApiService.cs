using BackEnd.Domain.Interface;
using BackEnd.DTO;
using static BackEnd.DTO.AwesomeApiContracts;

namespace BackEnd.Service;

public class AwesomeApiService(HttpClient http) : IExchangeRateService
{
    public async Task<QuoteResponse?> GetQuoteResponseAsync(string pair)
    {
        var data = await http.GetFromJsonAsync<Dictionary<string, QuoteResponse>>($"json/last/{pair}");
    
        return data?.GetValueOrDefault(pair.Replace("-", ""));
    
    }
}