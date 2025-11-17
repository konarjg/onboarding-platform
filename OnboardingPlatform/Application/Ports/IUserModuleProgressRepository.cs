namespace Application.Ports;

using Domain;
using Dtos;

public interface IUserModuleProgressRepository {
  Task<UserModuleProgress?> GetByUserAndModuleAsync(int userId, int moduleId, CancellationToken cancellationToken = default);
  Task<int> GetCompletedCountForUserAndPathAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  Task<List<ModuleCompletionStatus>> GetStatusesForUserAndPathAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  void Add(UserModuleProgress progress);
}
