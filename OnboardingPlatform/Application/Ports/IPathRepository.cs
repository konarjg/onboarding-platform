namespace Application.Ports;

using Domain;

public interface IPathRepository {
  Task<Path?> GetByIdAsync(int pathId,
    CancellationToken cancellationToken = default);
  Task<PagedResult<Path>> GetAllAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default);
  void Add(Path path);
  void Delete(Path path);
}
