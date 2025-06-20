using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class RateApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public RateApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

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
    public async Task GetRate_Authorized_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/rate");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("value", json);
    }

    [Fact]
    public async Task GetRate_Unauthorized_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/rate");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRate_Authorized_ChangesValue()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var content = new StringContent(JsonSerializer.Serialize(new { value = 123 }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/rate", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("123", json);
    }
} 