using Microsoft.Extensions.DependencyInjection;
using AdminDashboard.Api.Data.Repositories;
using AdminDashboard.Api.Domain.Interfaces;
using AdminDashboard.Api.Auth;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Api.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, string connectionString, JwtSettings jwtSettings)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string must not be null or empty.");
            if (jwtSettings == null)
                throw new ArgumentNullException(nameof(jwtSettings), "JwtSettings must not be null.");
            services.AddDbContext<Data.AppDbContext>(options =>
                options.UseSqlite(new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            );
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IRateRepository, RateRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton(jwtSettings);
            services.AddSingleton<JwtProvider>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<AuthService>();
        }
    }
} 