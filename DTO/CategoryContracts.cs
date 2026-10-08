using BackEnd.Domain.Entities;

namespace BackEnd.DTO;


public class CategoryContracts
{
    public record CategoryResponse(int Id, string Name);
    public record CreateCategoryRequest(string Name);
    public record CategorySummary( Category? Category, decimal Total, int Count );
    public record SummaryResponse( decimal Total, int Count, DateOnly From, DateOnly To, string Label, List<CategorySummary> ByCategory);
}