namespace BackEnd.DTO;

public class ExpenseContracts
{
    public record CreateExpenseRequest(int CategoryId, string Description, decimal Amount, DateOnly Date);

    public record ExpenseResponse(int Id, int CategoryId, string Description, decimal Amount, DateOnly Date );

    public record EditExpenseRequest( int CategoryId, string Description, decimal Amount, DateOnly Date );


}