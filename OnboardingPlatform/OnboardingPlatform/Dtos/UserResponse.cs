namespace OnboardingPlatform.Dtos;

using Domain;

public record UserResponse(int Id, string Email, UserRole Role, bool IsActive);
