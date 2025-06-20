using AdminDashboard.Api.Domain.Entities;
using AdminDashboard.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Api.Data.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _db;
    public ClientRepository(AppDbContext db) => _db = db;

    public async Task<List<Client>> GetAllAsync() => await _db.Clients.Include(c => c.Labels).AsNoTracking().ToListAsync();
    public async Task<Client?> GetByIdAsync(int id) => await _db.Clients.Include(c => c.Labels).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    public async Task AddAsync(Client client)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
    }
    public async Task UpdateAsync(Client client)
    {
        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client != null)
        {
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
        }
    }
} 