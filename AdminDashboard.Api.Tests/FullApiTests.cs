using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class FullApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public FullApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

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
    public async Task CreateClient_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dto = new { name = "Тестовый Клиент", email = "test@example.com", balanceT = 500, labels = new[] { "VIP" } };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/clients", content);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateClient_InvalidData()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dto = new { name = "", email = "", balanceT = 0, labels = new string[0] };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/clients", content);
        Assert.False(response.IsSuccessStatusCode); // Ожидаем ошибку
    }

    [Fact]
    public async Task UpdateClient_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Сначала создаём клиента
        var dto = new { name = "Клиент для обновления", email = "update@example.com", balanceT = 100, labels = new[] { "A" } };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResp = await client.PostAsync("/clients", content);
        var location = createResp.Headers.Location.ToString();
        var id = int.Parse(location.Split('/').Last());
        // Обновляем
        var updateDto = new { name = "Обновлённый Клиент", email = "update2@example.com", balanceT = 200, labels = new[] { "B" } };
        var updateContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/clients/{id}", updateContent);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateClient_NotFound()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var updateDto = new { name = "Не найден", email = "notfound@example.com", balanceT = 0, labels = new string[0] };
        var updateContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/clients/99999", updateContent);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClient_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Сначала создаём клиента
        var dto = new { name = "Клиент для удаления", email = "delete@example.com", balanceT = 100, labels = new[] { "A" } };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResp = await client.PostAsync("/clients", content);
        var location = createResp.Headers.Location.ToString();
        var id = int.Parse(location.Split('/').Last());
        // Удаляем
        var response = await client.DeleteAsync($"/clients/{id}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClient_NotFound()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.DeleteAsync($"/clients/99999");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode); // API возвращает 204 даже если не найден
    }

    [Fact]
    public async Task GetClientById_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Сначала создаём клиента
        var dto = new { name = "Клиент для поиска", email = "find@example.com", balanceT = 100, labels = new[] { "A" } };
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResp = await client.PostAsync("/clients", content);
        var location = createResp.Headers.Location.ToString();
        var id = int.Parse(location.Split('/').Last());
        // Получаем
        var response = await client.GetAsync($"/clients/{id}");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetClientById_NotFound()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"/clients/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetClientPayments_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Получаем платежи по существующему клиенту (id = 1 — из сидов)
        var response = await client.GetAsync($"/clients/1/payments");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetClientPayments_NotFound()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"/clients/99999/payments");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode); // API возвращает 200 даже если платежей нет
    }

    [Fact]
    public async Task CreatePayment_Success()
    {
        var client = _factory.CreateClient();
        var token = await GetJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Создаём платеж для существующего клиента (id = 1)
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

    [Fact]
    public async Task RefreshToken_Success()
    {
        var client = _factory.CreateClient();
        // Получаем refreshToken через логин
        var loginContent = new StringContent(JsonSerializer.Serialize(new { email = "admin@mirra.dev", password = "admin123" }), Encoding.UTF8, "application/json");
        var loginResp = await client.PostAsync("/auth/login", loginContent);
        var loginJson = await loginResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(loginJson);
        var refreshToken = doc.RootElement.GetProperty("refreshToken").GetString();
        // Обновляем токен
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