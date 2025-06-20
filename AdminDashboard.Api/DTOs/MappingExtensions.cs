using AdminDashboard.Api.Domain.Entities;

namespace AdminDashboard.Api.DTOs;

public static class MappingExtensions
{
    public static ClientDto ToDto(this Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            BalanceT = client.BalanceT,
            Labels = client.Labels?.Select(l => l.Name).ToList() ?? new List<string>()
        };
    }

    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            ClientId = payment.ClientId,
            ClientName = payment.Client?.Name ?? string.Empty,
            Amount = payment.Amount,
            Date = payment.Date
        };
    }

    public static RateDto ToDto(this Rate rate)
    {
        return new RateDto
        {
            Value = rate.Value,
            UpdatedAt = rate.UpdatedAt
        };
    }
} 