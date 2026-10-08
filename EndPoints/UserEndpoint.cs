using System.Security.Claims;
using BackEnd.Data;
using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static BackEnd.DTO.AuthContracts;

namespace BackEnd.EndPoints;


public static class UserEndpoint
{
    public static IEndpointRouteBuilder MapUserEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet("/users/me", Profile ).RequireAuthorization();
        return app;
    }
    
    private static async Task<IResult> Profile(ClaimsPrincipal user, AppDbContext db)
    {
        var userId = GetCurrentUser(user);
        if( userId is null )
            return Results.Unauthorized();

        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == userId);
        if( profile is null )
            return Results.NotFound();

        
        return Results.Ok(ToResponse(profile));
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse(user.Id, user.Email, user.Name);
    }

    private static int? GetCurrentUser(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim is not null && int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}