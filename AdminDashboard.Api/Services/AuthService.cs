using AdminDashboard.Api.Domain.Interfaces;
using AdminDashboard.Api.DTOs;
using AdminDashboard.Api.Auth;
using BCrypt.Net;

namespace AdminDashboard.Api.Services;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly JwtProvider _jwt;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepo, IRefreshTokenRepository refreshRepo, JwtProvider jwt, JwtSettings jwtSettings)
    {
        _userRepo = userRepo;
        _refreshRepo = refreshRepo;
        _jwt = jwt;
        _jwtSettings = jwtSettings;
    }

    public async Task<LoginResponseDto?> LoginAsync(string email, string password)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        var refresh = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
        };
        await _refreshRepo.AddAsync(refresh);
        return new LoginResponseDto { Token = accessToken, RefreshToken = refreshToken };
    }

    public async Task<LoginResponseDto?> RefreshAsync(string refreshToken)
    {
        var refresh = await _refreshRepo.GetByTokenAsync(refreshToken);
        if (refresh == null)
            return null;
        await _refreshRepo.RevokeAsync(refresh);
        var user = refresh.User;
        var accessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();
        var newRefresh = new Domain.Entities.RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
        };
        await _refreshRepo.AddAsync(newRefresh);
        return new LoginResponseDto { Token = accessToken, RefreshToken = newRefreshToken };
    }
} 