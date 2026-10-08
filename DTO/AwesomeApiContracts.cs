using System.Text.Json.Serialization;

namespace BackEnd.DTO;

public class AwesomeApiContracts
{

    public record QuoteResponse(string Code, string CodeIn, string Name,
     string High, string Low, string VarBid,
      string PctChange, string Bid, string Ask, string Timestamp,
      [property: JsonPropertyName("create_date")] string CreateDate );

      
}