namespace OnboardingPlatform.Configuration;

using Application;
using Application.Interfaces;

public static class ApplicationConfiguration {
  public static IServiceCollection AddApplication(this IServiceCollection services) {
    services.AddScoped<IAuthService,AuthService>();
    services.AddScoped<IUserService,UserService>();
    services.AddScoped<ILearningService,LearningService>();
    services.AddScoped<IContentManagementService,ContentManagementService>();

    return services;
  }
}
