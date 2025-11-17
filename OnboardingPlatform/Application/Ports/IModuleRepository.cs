namespace Application.Ports;

using Domain;

public interface IModuleRepository {
  Task<Module?> GetByIdAsync(int moduleId, CancellationToken cancellationToken = default);
  Task<PagedResult<Module>> GetForPathAsync(int pathId, int? pointer, int pageSize, CancellationToken cancellationToken = default);
  void Add(Module module);
  void Delete(Module module);
  Task<int> GetCountForPathAsync(int pathId, CancellationToken cancellationToken = default);
}
