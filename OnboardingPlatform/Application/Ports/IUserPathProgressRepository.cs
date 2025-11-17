namespace Application.Ports;

using Domain;
using Dtos;

public interface IUserPathProgressRepository {
  Task<UserPathProgress?> GetByUserAndPathAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  Task<UserPathProgress?> GetByUserAndPathWithDetailsAsync(int userId, int pathId, CancellationToken cancellationToken = default);
  Task<PagedResult<UserPathProgress>> GetForUserAsync(int userId, int? pointer, int pageSize, CancellationToken cancellationToken = default);
  void Add(UserPathProgress progress);
  void Delete(UserPathProgress progress);
  Task<PagedResult<UserPathProgressWithCounts>> GetForUserWithProgressCountsAsync(int userId, int? pointer, 
    int pageSize, CancellationToken cancellationToken = default);
}
