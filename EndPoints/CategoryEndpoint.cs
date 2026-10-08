using System.Security.Claims;
using BackEnd.Data;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static BackEnd.DTO.CategoryContracts;

namespace BackEnd.EndPoints;




public static class CategoryEndpoint
{
    public static IEndpointRouteBuilder MapCategoryEndpoint( this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/category").RequireAuthorization();
        group.MapGet("", List);
        group.MapPost("/create", Create);
        group.MapDelete("/delete/{id:int}", Delete);
        return app;
    }


    private static async Task<IResult> List(AppDbContext db, ClaimsPrincipal user)
    {
        var currentUser = GetCurrentUser( user );
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }
        var response = await db.Categories.Where(x => x.UserId == null || x.UserId == currentUser).Select(x => ToResponse(x)).ToArrayAsync();
        return Results.Ok(response);
    }
    private static async Task<IResult> Create(ClaimsPrincipal user, AppDbContext db, CreateCategoryRequest request)
    {
        var currentUser = GetCurrentUser(user);
        if(currentUser is null)
        {
            return Results.Unauthorized();
        }


        var exists = await db.Categories.AnyAsync( x => x.UserId == currentUser && x.Name == request.Name );
        if (exists)
        {
            return Results.Conflict();
        }

        var category = Category.Create(currentUser, request.Name);
        
        db.Add(category);
        await db.SaveChangesAsync();
        return Results.Created($"/created/{category.Id}", category.Id);


    }
    private static async Task<IResult> Delete(int Id, ClaimsPrincipal user, AppDbContext db)
    {
        var currentUser = GetCurrentUser(user);
        if(currentUser is null)
        {
            return Results.Unauthorized();
        }

        var category = await db.Categories.FirstOrDefaultAsync( x => x.Id == Id && x.UserId == currentUser);
        if(category is null)
        {
            return Results.NotFound();
        }
        db.Remove(category);
        await db.SaveChangesAsync();
        return Results.NoContent();

    }
    private static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(category.Id, category.Name);
    }


    private static int? GetCurrentUser(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim is not null && int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}