namespace AdminDashboard.Api.Auth;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int AccessTokenLifetimeMinutes { get; set; } = 30;
    public int RefreshTokenLifetimeDays { get; set; } = 7;
}
 