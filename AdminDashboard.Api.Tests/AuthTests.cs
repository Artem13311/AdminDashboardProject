using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public AuthTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Login_Success_ReturnsToken()
    {
        var client = _factory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(new { email = "admin@mirra.dev", password = "admin123" }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/auth/login", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("token", out _));
    }

    [Fact]
    public async Task Login_Fail_Returns401()
    {
        var client = _factory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(new { email = "admin@mirra.dev", password = "wrong" }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/auth/login", content);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_Success()
    {
        var client = _factory.CreateClient();
        var loginContent = new StringContent(JsonSerializer.Serialize(new { email = "admin@mirra.dev", password = "admin123" }), Encoding.UTF8, "application/json");
        var loginResp = await client.PostAsync("/auth/login", loginContent);
        var loginJson = await loginResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(loginJson);
        var refreshToken = doc.RootElement.GetProperty("refreshToken").GetString();
        var refreshContent = new StringContent(JsonSerializer.Serialize(new { refreshToken }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/auth/refresh", refreshContent);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_Invalid()
    {
        var client = _factory.CreateClient();
        var refreshContent = new StringContent(JsonSerializer.Serialize(new { refreshToken = "invalid" }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/auth/refresh", refreshContent);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 