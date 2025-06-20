namespace AdminDashboard.Api.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal BalanceT { get; set; }
    public List<Payment> Payments { get; set; } = new();
    public List<ClientLabel> Labels { get; set; } = new();
} 