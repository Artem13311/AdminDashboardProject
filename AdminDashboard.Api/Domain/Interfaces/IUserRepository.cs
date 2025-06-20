using AdminDashboard.Api.Domain.Entities;

namespace AdminDashboard.Api.Domain.Interfaces;
 
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
} 