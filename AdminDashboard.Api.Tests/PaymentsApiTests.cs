using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class PaymentsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public PaymentsApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private async Task<string> GetJwtTokenAsync()
    {
        var client = _factory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(new { email = "admin@mirra.dev", password = "admin123" }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/auth/login", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString();
    }

    [Fact]
    public async Task GetPayments_Authorized_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/payments?take=3");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("amount", json); // простая проверка
    }

    [Fact]
    public async Task GetPayments_Unauthorized_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/payments?take=3");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePayment_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dto = new { clientId = 1, amount = 123, date = DateTime.UtcNow };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/payments", content);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreatePayment_InvalidClient()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dto = new { clientId = 99999, amount = 123, date = DateTime.UtcNow };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/payments", content);
        Assert.False(response.IsSuccessStatusCode); // Ожидаем ошибку
    }
} 