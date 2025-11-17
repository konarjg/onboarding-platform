namespace Application.Interfaces;

using Domain;
using Domain.Commands;
using Dtos;

public interface ILearningService {
  Task<UserPathProgress> EnrollUserInPathAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  Task UnenrollUserFromPathAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  Task<PagedResult<UserPathWithProgress>> GetUserAssignedPathsAsync(int userId, int? pointer, int pageSize, CancellationToken cancellationToken = default);
  Task<UserPathDetails?> GetUserAssignedPathDetailsAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  Task<UserModuleProgress?> UpdateModuleProgressAsync(int userId, int moduleId, UpdateModuleProgressCommand command, CancellationToken cancellationToken = default);
  Task<UserModuleProgress?> GetModuleProgressForUserAsync(int userId, int moduleId, CancellationToken cancellationToken = default);
}
