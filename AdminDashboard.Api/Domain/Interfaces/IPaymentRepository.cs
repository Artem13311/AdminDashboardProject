using AdminDashboard.Api.Domain.Entities;

namespace AdminDashboard.Api.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<List<Payment>> GetLastAsync(int take);
    Task<List<Payment>> GetByClientIdAsync(int clientId);
    Task AddAsync(Payment payment);
} 