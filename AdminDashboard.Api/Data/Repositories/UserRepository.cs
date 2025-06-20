using AdminDashboard.Api.Domain.Entities;
using AdminDashboard.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Api.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
} 