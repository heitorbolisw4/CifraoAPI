using BackEnd.Domain;
using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories {get; set;}
    public DbSet<Expense> Expenses {get; set;}
    public DbSet<ExchangeRate> ExchangeRates { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
           entity.HasKey(x => x.Id);

           entity.Property(x => x.Name).IsRequired().HasMaxLength(50);

           entity.HasIndex(x => x.Email).IsUnique();
           entity.Property(x => x.Email).IsRequired().HasMaxLength(150);
           entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255); 
        });

        modelBuilder.Entity<Category>( entity =>
        {
            entity.HasKey( x => x.Id);

            entity.HasIndex( x => new {x.UserId, x.Name}).IsUnique();
        
            entity.Property( x => x.Name ).IsRequired().HasMaxLength(30);
            entity.Property( x => x.IsSystem).IsRequired();

            entity.HasOne( x => x.User ).WithMany( y => y.Categories ).HasForeignKey(x => x.UserId );
            
            
            
            entity.HasData(
                    new { Id = 1, Name = "Alimentacao", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 2, Name = "Transporte", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 3, Name = "Moradia", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 4, Name = "Saude", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 5, Name = "Educacao", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 6, Name = "Lazer", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 7, Name = "Outros", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)},
                    new { Id = 11, Name = "Outros", IsSystem = true, CreatedAt = new DateTime(2026, 9, 15)}
            );

        });

        modelBuilder.Entity<Expense>( entity =>
        {
           entity.HasKey( x => x.Id);

           entity.Property( x => x.Description ).IsRequired().HasMaxLength(255);

           entity.Property( x => x.Amount ).HasColumnType("decimal(18, 2)").IsRequired();

           entity.HasOne( x => x.Category).WithMany( y => y.Expenses ).HasForeignKey( x => x.CategoryId ); 
           entity.HasOne( x => x.User).WithMany( y => y.Expenses).HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<ExchangeRate>( entity =>
        {
            entity.HasKey( x => x.Id);
            entity.Property( x => x.BaseCurrency ).HasMaxLength(3);
            entity.Property( x => x.QuoteCurrency ).HasMaxLength(3);
            entity.Property( x => x.Bid ).HasColumnType("decimal(18,6)");
            entity.Property( x => x.Ask ).HasColumnType("decimal(18,6)");
            entity.Property( x => x.Source ).IsRequired().HasMaxLength(30);

            entity.HasIndex(x =>  new {x.BaseCurrency, x.QuoteCurrency, x.RateDate}).IsUnique();
        });
    }
}