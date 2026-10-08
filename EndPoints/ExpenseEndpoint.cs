using System.Security.Claims;
using BackEnd.Data;
using BackEnd.Domain;
using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static BackEnd.Domain.ExpenseSummaryCalculator;
using static BackEnd.DTO.ExpenseContracts;

namespace BackEnd.EndPoints;



public static class ExpenseEndpoint
{
    public static IEndpointRouteBuilder MapExpenseEndpoints( this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/expenses").RequireAuthorization();
        group.MapPost("", Create);
        group.MapGet("", GetAll);
        group.MapGet("/summary", SummaryExpenses);
        group.MapGet("/{id:int}", GetById);
        group.MapDelete("/{id:int}", Delete);
        group.MapPut("/{id:int}", Edit);

        return app;
    }

    private static  async Task<IResult> Create(AppDbContext db, ClaimsPrincipal user, CreateExpenseRequest request)
    {

        
        var currentUser = GetCurrentUser(user);
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }

        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == currentUser);
        if( profile is null)
        {
            return Results.Unauthorized();
        }


        var category = await db.Categories.FirstOrDefaultAsync( x => x.Id == request.CategoryId );
        if(category is null)
        {
            return Results.BadRequest();
        }
        

        var expense = Expense.Create(profile.Id, request.CategoryId, request.Description, request.Amount, request.Date);

        db.Add(expense);
        await db.SaveChangesAsync();

        return Results.Created($"{expense.Id}", expense.Id);
    }

    private static async Task<IResult> GetAll(DateOnly? from, DateOnly? to, int? categoryId, AppDbContext db, ClaimsPrincipal user)
    {
        var currentUser = GetCurrentUser(user);
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }
        
        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == currentUser );
        if( profile is null)
        {
            return Results.Unauthorized();
        }
        
        var query =  db.Expenses.Where( x => x.UserId == profile.Id);        
        
        if( from > to)
        {
            return Results.BadRequest();
        } 


        if( from is not null )
        {
            query = query.Where( x => x.Date >= from );
        }
        if( to is not null)
        {
            query = query.Where( x => x.Date <= to );
        }
        if( categoryId is not null)
        {
            query = query.Where( x => x.CategoryId == categoryId);
        }
        
        var response = await query.Select( x => ToResponse(x)).ToListAsync();
        return Results.Ok(response);

    }

    private static async Task<IResult> Delete( int id, ClaimsPrincipal user, AppDbContext db)
    {
        // eu verifico o current user
        var currentUser = GetCurrentUser(user);
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }
        
        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == currentUser );
        if( profile is null )
        {
            return Results.Unauthorized();
        }
        
        // eu localizo a expense
        var expense = await db.Expenses.FirstOrDefaultAsync( x => x.Id == id && x.UserId == profile.Id );        
        if(expense is null)
        {
            return Results.NotFound();
        }

        db.Remove(expense);
        await db.SaveChangesAsync();

        return Results.NoContent();

    }
    private static async Task<IResult> Edit( int id, ClaimsPrincipal user, AppDbContext db, EditExpenseRequest request)
    {
        var currentUser = GetCurrentUser(user);
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }
        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == currentUser);
        if( profile is null)
        {
            return Results.Unauthorized();
        }

        // eu identifico a expense pelo id
        var expense = await db.Expenses.FirstOrDefaultAsync( x => x.Id == id && x.UserId == profile.Id );
        if( expense is null)
        {
            return Results.NotFound();
        }

        expense.Update(request.CategoryId, request.Description, request.Amount, request.Date);        


                
                
        await db.SaveChangesAsync();
        return Results.NoContent();

    }

    private static async Task<IResult> GetById(int id, ClaimsPrincipal user, AppDbContext db)
    {
        var currentUser = GetCurrentUser(user);
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }
        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == currentUser);
        if( profile is null)
        {
            return Results.Unauthorized();
        }

        var expense = await db.Expenses.FirstOrDefaultAsync( x => x.Id == id && x.UserId == profile.Id );
        if( expense is null)
        {
            return Results.NotFound();
        }

        var response = ToResponse(expense);
        return Results.Ok(response);

    }

    private static async Task<IResult> SummaryExpenses(string? period, ClaimsPrincipal user, AppDbContext db)
    {
        var currentUser = GetCurrentUser(user);
        if( currentUser is null)
        {
            return Results.Unauthorized();
        }
        var profile = await db.Users.FirstOrDefaultAsync( x => x.Id == currentUser);
        if( profile is null)
        {
            return Results.Unauthorized();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        ExpenseWindow? window = ResolvePeriod(period, today);
        if( window is null)
        {
            return Results.BadRequest("Invalid Period");
        }        
        var query = await db.Expenses.Where( x => x.UserId == profile.Id && x.Date >= window.Start && x.Date <= window.End).ToListAsync();
        var response = Sumarize(query, window);
        return Results.Ok(response);
    }


    private static ExpenseResponse ToResponse(Expense expense)
    {
        return new ExpenseResponse(expense.Id,expense.CategoryId, expense.Description, expense.Amount, expense.Date);
    }







    private static int? GetCurrentUser( ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim is not null && int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

}