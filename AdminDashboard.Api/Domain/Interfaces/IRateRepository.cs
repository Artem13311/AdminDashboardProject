using AdminDashboard.Api.Domain.Entities;

namespace AdminDashboard.Api.Domain.Interfaces;
 
public interface IRateRepository
{
    Task<Rate?> GetCurrentAsync();
    Task UpdateAsync(decimal newValue);
} 