namespace Application.Dtos;

using Domain;

public record UserPathWithProgress(
  Path Path,
  double ProgressPercentage
);

public record ModuleCompletionStatus(
  int ModuleId,
  string Title,
  bool IsCompleted
);

public record UserPathDetails(
  Path Path,
  UserPathProgress EnrollmentDetails,
  ICollection<ModuleCompletionStatus> ModuleStatuses
);