namespace OnboardingPlatform.Configuration;

using Application.Ports;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureConfiguration {
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
    services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    
    string? connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<DatabaseContext>(options => 
      options.UseSqlite(connectionString));
    
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IPathRepository, PathRepository>();
    services.AddScoped<IModuleRepository, ModuleRepository>();
    services.AddScoped<IContentSectionRepository, ContentSectionRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<IUserPathProgressRepository, UserPathProgressRepository>();
    services.AddScoped<IUserModuleProgressRepository, UserModuleProgressRepository>();
    
    services.AddSingleton<IPasswordHasher, PasswordHasher>();
    services.AddSingleton<ITokenGenerator, TokenGenerator>();
    
    return services;
  }
}
