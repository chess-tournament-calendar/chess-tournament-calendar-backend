using ChessTournamentCalendarBackend.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChessTournamentCalendarBackend.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetItem> BudgetItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .Property(u => u.Title)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
        

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.Budget)
            .WithOne(b => b.Tournament)
            .HasForeignKey<Budget>(b => b.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Tournament>()
            .Property(t => t.Type)
            .HasConversion<string>();


        modelBuilder.Entity<Budget>()
            .HasMany(b => b.Items)
            .WithOne(i => i.Budget)
            .HasForeignKey(i => i.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Budget>()
            .Property(b => b.PlanType)
            .HasConversion<string>();
            
        modelBuilder.Entity<Budget>()
            .Property(b => b.Currency)
            .HasConversion<string>();
        
        modelBuilder.Entity<BudgetItem>()
            .Property(bi => bi.Currency)
            .HasConversion<string>();


    }
    
}