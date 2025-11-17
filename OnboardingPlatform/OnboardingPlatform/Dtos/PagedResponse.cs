namespace OnboardingPlatform.Dtos;

public record PagedResponse<T>(List<T> Items, int? NextPointer, bool HasNextPage);
