using Microsoft.EntityFrameworkCore;
using AdminDashboard.Api.Domain.Entities;

namespace AdminDashboard.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Rate> Rates => Set<Rate>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ClientLabel> ClientLabels => Set<ClientLabel>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
} 