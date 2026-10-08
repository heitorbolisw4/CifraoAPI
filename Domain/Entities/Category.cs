using BackEnd.Domain.Exceptions;

namespace BackEnd.Domain.Entities;



public class Category
{
    private Category(int? userId, string name, bool isSystem, DateTime createdAt)
    {
        UserId = userId;
        Name = name;
        IsSystem = isSystem;
        CreatedAt = createdAt;
    }

    public int Id { get; private set; }
    public int? UserId { get; private set; }
    public string Name { get; private set; }
    public bool IsSystem { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public User? User { get; set; }
    public List<Expense>? Expenses { get; set; }
    

    public static Category Create( int? userId, string name )
    {
        ValidUserId(userId);
        var categoryName = ValidCategoryName(name);
        var isSistem = false;
        
        return new Category(userId, categoryName, isSistem, DateTime.UtcNow);
    }
    private static string ValidCategoryName(string name)
    {
        if( string.IsNullOrWhiteSpace( name ) || name.Length < 3 || name.Length > 40)
            throw new DomainException("the category name cannot be empty or shorther than 3 characters. ");
        

        string categoryName = name.Trim()[..1].ToUpperInvariant() + name[1..].ToLowerInvariant();
        return categoryName;
    }
    private static int? ValidUserId(int? userId)
    {
        if( userId <= 0)
        {
            throw new DomainException("An issue occurred while accessing the userId");
        }
        return userId;
    }

    
}