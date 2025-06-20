using AdminDashboard.Api.Domain.Entities;
using AdminDashboard.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Api.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;
    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public async Task<RefreshToken?> GetByTokenAsync(string token) =>
        await _db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow);

    public async Task AddAsync(RefreshToken refreshToken)
    {
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();
    }

    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        _db.RefreshTokens.Update(refreshToken);
        await _db.SaveChangesAsync();
    }
} 