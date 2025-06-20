using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Api.DTOs;

public class ClientDto
{
    public int Id { get; set; }
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public decimal BalanceT { get; set; }
    public List<string> Labels { get; set; } = new();
} 