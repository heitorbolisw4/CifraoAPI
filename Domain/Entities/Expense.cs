using BackEnd.Domain.Entities;
using BackEnd.Domain.Exceptions;

namespace BackEnd.Domain;


public class Expense
{
    private Expense(int userId, int categoryId, string description, decimal amount, DateOnly date)
    {
        UserId = userId;
        CategoryId = categoryId;
        Description = description;
        Amount = amount;
        Date = date;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int CategoryId { get; private set; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly Date { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateOnly UpdatedAt { get; private set; }
    public Category? Category { get; set; }
    public User? User { get; set; }

    public static Expense Create(int userId, int categoryId, string description, decimal amount, DateOnly date)
    {
        ValidUserId( userId );
        ValidCategoryId( categoryId );
        ValidDescription( description );
        ValidAmount( amount );
        ValidDate( date );
        return new Expense(userId, categoryId ,description, amount, date );

    }

    public  void Update(int categoryId, string description, decimal amount, DateOnly date)
    {
        ValidCategoryId(categoryId);
        ValidAmount(amount);
        ValidDate( date );
        CategoryId = categoryId;
        Description = ValidDescription(description);
        Amount = amount;
        Date = date;
    }



    private static void ValidUserId(int id)
    {
        if( id <= 0)
        {
            throw new DomainException("Invalid User");
        }
        
    }

    private static string ValidDescription( string description )
    {
        if( string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Invalid Description");
        }
        var _description = description.Trim();
        return _description;

    }

    private static void ValidAmount( decimal amount )
    {
        if( amount <= 0m)
        {
            throw new DomainException("Invalid Amount");
        }
    }
    private static void ValidDate( DateOnly date)
    {
        var validDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        if( date > validDate)
        {
            throw new DomainException("Invalid Date");
        }
        
    }
    private static void ValidCategoryId( int categoryId)
    {
        if( categoryId <= 0)
        {
            throw new DomainException("Invalid Category");
        }
    }


}