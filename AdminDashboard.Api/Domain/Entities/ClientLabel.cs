namespace AdminDashboard.Api.Domain.Entities;

public class ClientLabel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
} 