namespace OnboardingPlatform.Configuration;

using Application.Ports;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

public static class AdministrationSeed
{
    public static async Task SeedUsersAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<DatabaseContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            
            await context.Database.EnsureCreatedAsync();
            
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new() {
                        Email = "admin@platform.com",
                        PasswordHash = passwordHasher.HashPassword("Admin123!"),
                        Role = UserRole.Admin,
                        IsActive = true
                    },
                    new() {
                        Email = "hr@platform.com",
                        PasswordHash = passwordHasher.HashPassword("Hr123!"),
                        Role = UserRole.HumanResources,
                        IsActive = true
                    },
                    new() {
                        Email = "manager@platform.com",
                        PasswordHash = passwordHasher.HashPassword("Manager123!"),
                        Role = UserRole.Manager,
                        IsActive = true
                    }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
                
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Database seeded successfully with 3 default users.");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}