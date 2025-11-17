namespace Application.Dtos;

using Domain;

public record UserPathProgressWithCounts(
  UserPathProgress Enrollment,
  int TotalModuleCount,
  int CompletedModuleCount);
