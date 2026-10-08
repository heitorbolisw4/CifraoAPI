using static BackEnd.DTO.CategoryContracts;

namespace BackEnd.Domain;


public static class ExpenseSummaryCalculator
{
    public record ExpenseWindow(DateOnly Start, DateOnly End, string Label);

    public static ExpenseWindow? ResolvePeriod( string? period, DateOnly today)
    {
        var aux = period;
        if (string.IsNullOrWhiteSpace(aux))
        {
            aux = "month";
        }

        var DefaultPeriod = aux.Trim().ToLowerInvariant();

        DateOnly start;
        string label;

        switch (DefaultPeriod)
        {
            case "week":
                start = today.AddDays(-7);
                label = "Last 7 days";
                break;

            case "month":
                start = today.AddMonths(-1);
                label = "Last 30 days";
                break;
            
            case "3months":
                start = today.AddMonths(-3);
                label = "Last 3 months";
                break;
            case "year":
                start = today.AddYears(-1);
                label = "Last 12 months";
                break;
            case "all":
                start = DateOnly.MinValue;
                label = "All time";
                break;
                
            default:
                return null;
        }
        return new ExpenseWindow(start, today, label);
    }

    public static SummaryResponse Sumarize(IEnumerable<Expense> expenses, ExpenseWindow window)
    {
        var byCategory = expenses.GroupBy( g => g.Category ).Select( x => new CategorySummary
        (
            Category: x.Key,
            Total: x.Sum( e => e.Amount),
            Count: x.Count()
        )).OrderByDescending(x => x.Total).ToList();
        var total = byCategory.Sum(x => x.Total);
        var count = byCategory.Sum(x => x.Count);
        return new SummaryResponse(total, count, window.Start, window.End, window.Label, byCategory);
    }




}