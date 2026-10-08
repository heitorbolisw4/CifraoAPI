using BackEnd.Data;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Exceptions;
using BackEnd.Domain.Interface;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using static BackEnd.DTO.AuthContracts;

namespace BackEnd.EndPoints;


public static class AuthEndpoint
{
    public static IEndpointRouteBuilder MapAuthEndpoints( this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", Register);
        app.MapPost("/auth/login", Login);
        return app;
    }    

    public static async Task<IResult> Register(UserRegisterRequest request, AppDbContext db, IAuthService service)
    {
        
        if(string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8 || request.Password.Length > 255 )
        {
            return Results.BadRequest("your must enter a valid password");
        }
        
        if(string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.BadRequest("you must fill all places");
        }
        var email = request.Email.Trim().ToLowerInvariant();
        var name = request.Name.Trim();

        if(await db.Users.AnyAsync(x => x.Email == email))
        {
            return Results.Conflict("Email already registered");
        }

        var doHashPassw = service.PasswordHasher(request.Password);

        var user = User.Create(name, email, doHashPassw);


        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync();
                 
        }
        catch(DbUpdateException ex)
        {
            if(ex.InnerException is SqliteException sqliteEx)
            {
                switch (sqliteEx.SqliteExtendedErrorCode)
                {
                    case 2067:
                        return Results.Conflict( new { error = "Violates the UNIQUE constraint"});
                    case 5:
                        return Results.StatusCode(503);
                    default:
                        return Results.BadRequest( new { error = $"SQLite error({sqliteEx.SqliteExtendedErrorCode})"});
                }
            }
            return Results.BadRequest( new { error = "Error updating the database", details = ex.Message});
        }

        return Results.Created( $"/users/{user.Id}", user.Id );
        
    }



    // autenticar um usuario
    public static async Task<IResult> Login( UserLoginRequest request, AppDbContext db, IAuthService authService, ITokenService tokenService )
    {
        if(string.IsNullOrWhiteSpace( request.Email ) || string.IsNullOrWhiteSpace( request.Password ) )
        {
            return Results.BadRequest( "you must fill all places" );
        }
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        
        var user = await db.Users.FirstOrDefaultAsync( x => x.Email == normalizedEmail );
        if(user is null || !authService.PasswordVerify( request.Password, user.PasswordHash ) )
        {
            return Results.Unauthorized();
        }

        var token = tokenService.GenerateToken( user );

        return Results.Ok(token);
    }


    
}
