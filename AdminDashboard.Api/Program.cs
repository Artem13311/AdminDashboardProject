using AdminDashboard.Api.Data;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Api.Data.Repositories;
using AdminDashboard.Api.Domain.Interfaces;
using AdminDashboard.Api.Domain.Entities;
using AdminDashboard.Api.DTOs;
using BCrypt.Net;
using AdminDashboard.Api.Auth;
using AdminDashboard.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.ConfigureApplicationServices(connectionString, jwtSettings);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(origins ?? new string[0])
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/login", async (LoginRequestDto req, AuthService authService) =>
{
    var result = await authService.LoginAsync(req.Email, req.Password);
    return result == null ? Results.Unauthorized() : Results.Ok(result);
});

app.MapPost("/auth/refresh", async (RefreshRequestDto req, AuthService authService) =>
{
    var result = await authService.RefreshAsync(req.RefreshToken);
    return result == null ? Results.Unauthorized() : Results.Ok(result);
});

app.MapGet("/clients", async (IClientRepository repo) =>
{
    var clients = await repo.GetAllAsync();
    return Results.Ok(clients.Select(c => c.ToDto()));
}).RequireAuthorization();

app.MapGet("/payments", async (int take, IPaymentRepository repo) =>
{
    var payments = await repo.GetLastAsync(take);
    return Results.Ok(payments.Select(p => p.ToDto()));
}).RequireAuthorization();

app.MapGet("/rate", async (IRateRepository repo) =>
{
    var rate = await repo.GetCurrentAsync();
    if (rate == null) return Results.NotFound();
    return Results.Ok(rate.ToDto());
}).RequireAuthorization();

app.MapPost("/rate", async (RateDto dto, IRateRepository repo) =>
{
    await repo.UpdateAsync(dto.Value);
    var rate = await repo.GetCurrentAsync();
    return Results.Ok(rate?.ToDto());
}).RequireAuthorization();

app.MapGet("/clients/{id}", async (int id, IClientRepository repo) =>
{
    var client = await repo.GetByIdAsync(id);
    return client == null ? Results.NotFound() : Results.Ok(client.ToDto());
}).RequireAuthorization();

app.MapPost("/clients", async (ClientDto dto, IClientRepository repo, HttpContext httpContext) =>
{
    var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
    var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto, null, null);
    if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto, context, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
    }
    var client = new Client
    {
        Name = dto.Name,
        Email = dto.Email,
        BalanceT = dto.BalanceT,
        Labels = dto.Labels.Select(l => new ClientLabel { Name = l }).ToList()
    };
    await repo.AddAsync(client);
    return Results.Created($"/clients/{client.Id}", client.ToDto());
}).RequireAuthorization();

app.MapPut("/clients/{id}", async (int id, ClientDto dto, IClientRepository repo, HttpContext httpContext) =>
{
    var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
    var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto, null, null);
    if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto, context, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
    }
    var client = await repo.GetByIdAsync(id);
    if (client == null) return Results.NotFound();
    client.Name = dto.Name;
    client.Email = dto.Email;
    client.BalanceT = dto.BalanceT;
    client.Labels = dto.Labels.Select(l => new ClientLabel { Name = l, ClientId = id }).ToList();
    await repo.UpdateAsync(client);
    return Results.Ok(client.ToDto());
}).RequireAuthorization();

app.MapDelete("/clients/{id}", async (int id, IClientRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
}).RequireAuthorization();

app.MapGet("/clients/{id}/payments", async (int id, IPaymentRepository repo) =>
{
    var payments = await repo.GetByClientIdAsync(id);
    return Results.Ok(payments.Select(p => p.ToDto()));
}).RequireAuthorization();

app.MapPost("/payments", async (PaymentDto dto, IPaymentRepository repo) =>
{
    var payment = new Payment
    {
        ClientId = dto.ClientId,
        Amount = dto.Amount,
        Date = dto.Date == default ? DateTime.UtcNow : dto.Date
    };
    await repo.AddAsync(payment);
    return Results.Created($"/payments/{payment.Id}", payment.ToDto());
}).RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.EnsureSeeded(db);
}

app.Run();

public partial class Program { }
