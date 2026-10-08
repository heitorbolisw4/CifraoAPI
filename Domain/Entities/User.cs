using System.Text.RegularExpressions;
using BackEnd.Domain.Exceptions;

namespace BackEnd.Domain.Entities;


public class User
{
    private User(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<Expense>? Expenses { get; set; }
    public List<Category>? Categories { get; set; }

    private static readonly Regex EmailRegex = new (@"^\S+@\S+\.\S+$", RegexOptions.Compiled);

    public static User Create( string name, string email, string passwordHash)
    {
        ValidName( name );
        ValidEmail( email );
        ValidPassword( passwordHash );
        return new User(name, email, passwordHash);
    }
    
    private static void ValidName( string name )
    {
        if( string.IsNullOrWhiteSpace( name ) || name.Length < 2 || name.Length > 50)
        {
            throw new DomainException("you must type a valid name");
        }
    }
    private static void ValidEmail( string email)
    {
        if( string.IsNullOrWhiteSpace( email ) || email.Length > 150 || !EmailRegex.IsMatch(email) )
        {
            throw new DomainException("you must type a valid email");
        }
    }
    private static void ValidPassword( string passwordHash)
    {
        if( string.IsNullOrWhiteSpace( passwordHash ) )
        {
            throw new DomainException("you must type a valid password");
        }
    }



}