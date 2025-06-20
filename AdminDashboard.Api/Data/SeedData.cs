using AdminDashboard.Api.Domain.Entities;
using BCrypt.Net;

namespace AdminDashboard.Api.Data;

public static class SeedData
{
    public static void EnsureSeeded(AppDbContext db)
    {
        db.Database.EnsureCreated();
        // Очищаем таблицы в правильном порядке, чтобы не нарушать внешние ключи
        db.Payments.RemoveRange(db.Payments);
        db.ClientLabels.RemoveRange(db.ClientLabels);
        db.RefreshTokens.RemoveRange(db.RefreshTokens);
        db.Clients.RemoveRange(db.Clients);
        db.Users.RemoveRange(db.Users);
        db.Rates.RemoveRange(db.Rates);
        db.SaveChanges();
        if (!db.Users.Any())
        {
            db.Users.Add(new User { Email = "admin@mirra.dev", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") });
            db.SaveChanges();
        }
        if (!db.Rates.Any())
        {
            db.Rates.Add(new Rate { Value = 10, UpdatedAt = DateTime.UtcNow });
            db.SaveChanges();
        }
        if (!db.Clients.Any())
        {
            var clients = new List<Client>
            {
                new Client { Name = "Иван Иванов", Email = "ivan@example.com", BalanceT = 100 },
                new Client { Name = "Петр Петров", Email = "petr@example.com", BalanceT = 200 },
                new Client { Name = "Сидор Сидоров", Email = "sidor@example.com", BalanceT = 300 }
            };
            db.Clients.AddRange(clients);
            db.SaveChanges();
            var dbClients = db.Clients.OrderBy(c => c.Id).ToList();
            db.Payments.AddRange(new[]
            {
                new Payment { ClientId = dbClients[0].Id, Amount = 10, Date = DateTime.UtcNow.AddDays(-1) },
                new Payment { ClientId = dbClients[1].Id, Amount = 20, Date = DateTime.UtcNow.AddDays(-2) },
                new Payment { ClientId = dbClients[2].Id, Amount = 30, Date = DateTime.UtcNow.AddDays(-3) },
                new Payment { ClientId = dbClients[0].Id, Amount = 40, Date = DateTime.UtcNow.AddDays(-4) },
                new Payment { ClientId = dbClients[1].Id, Amount = 50, Date = DateTime.UtcNow.AddDays(-5) },
            });
            db.SaveChanges();
        }
    }
} 