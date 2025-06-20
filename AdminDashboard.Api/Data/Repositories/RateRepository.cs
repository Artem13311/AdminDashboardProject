using AdminDashboard.Api.Domain.Entities;
using AdminDashboard.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Api.Data.Repositories;

public class RateRepository : IRateRepository
{
    private readonly AppDbContext _db;
    public RateRepository(AppDbContext db) => _db = db;

    public async Task<Rate?> GetCurrentAsync() => await _db.Rates.OrderByDescending(r => r.UpdatedAt).FirstOrDefaultAsync();

    public async Task UpdateAsync(decimal newValue)
    {
        var rate = await GetCurrentAsync();
        if (rate != null)
        {
            rate.Value = newValue;
            rate.UpdatedAt = DateTime.UtcNow;
            _db.Rates.Update(rate);
        }
        else
        {
            rate = new Rate { Value = newValue, UpdatedAt = DateTime.UtcNow };
            _db.Rates.Add(rate);
        }
        await _db.SaveChangesAsync();
    }
} 