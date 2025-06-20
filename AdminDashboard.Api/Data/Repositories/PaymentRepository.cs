using AdminDashboard.Api.Domain.Entities;
using AdminDashboard.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Api.Data.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;
    public PaymentRepository(AppDbContext db) => _db = db;

    public async Task<List<Payment>> GetLastAsync(int take) =>
        await _db.Payments.Include(p => p.Client).OrderByDescending(p => p.Date).Take(take).ToListAsync();

    public async Task<List<Payment>> GetByClientIdAsync(int clientId) =>
        await _db.Payments.Where(p => p.ClientId == clientId).OrderByDescending(p => p.Date).ToListAsync();

    public async Task AddAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
    }
} 