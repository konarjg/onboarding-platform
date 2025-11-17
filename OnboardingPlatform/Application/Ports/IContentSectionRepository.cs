namespace Application.Ports;

using Domain;

public interface IContentSectionRepository {
  Task<ContentSection?> GetByIdAsync(int contentSectionId, CancellationToken cancellationToken = default);
  Task<PagedResult<ContentSection>> GetForModuleAsync(int moduleId, int? pointer, int pageSize, CancellationToken cancellationToken = default);
  void Add(ContentSection contentSection);
  void Delete(ContentSection contentSection);
}
