namespace OnboardingPlatform.Dtos;

public record UserPathDetailsResponse(
  int PathId, 
  string Title, 
  string Summary,
  DateTime EnrollmentDate, 
  double ProgressPercentage,
  List<ModuleProgressResponse> Modules
);
