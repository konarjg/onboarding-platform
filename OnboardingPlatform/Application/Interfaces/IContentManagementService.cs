namespace Application.Interfaces;

using Domain;
using Domain.Commands;

public interface IContentManagementService {
  
  Task<Path?> GetPathByIdAsync(int pathId, CancellationToken cancellationToken = default);
  Task<Path?> UpdatePathAsync(int pathId, UpdatePathCommand command, CancellationToken cancellationToken = default);
  Task<PagedResult<Path>> GetAllPathsAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default);
  Task<Path> CreatePathAsync(CreatePathCommand command, CancellationToken cancellationToken = default);
  Task DeletePathAsync(int pathId, CancellationToken cancellationToken = default);

  Task<PagedResult<Module>> GetModulesForPathAsync(int pathId, int? pointer, int pageSize, CancellationToken cancellationToken = default);
  Task<Module> CreateModuleForPathAsync(int pathId, CreateModuleCommand command, CancellationToken cancellationToken = default);
  Task<Module?> GetModuleByIdAsync(int moduleId, CancellationToken cancellationToken = default);
  Task<Module?> UpdateModuleAsync(int moduleId, UpdateModuleCommand command, CancellationToken cancellationToken = default);
  Task DeleteModuleAsync(int moduleId, CancellationToken cancellationToken = default);

  Task<ContentSection?> GetContentSectionByIdAsync(int contentSectionId, CancellationToken cancellationToken = default);
  Task<PagedResult<ContentSection>> GetContentForModuleAsync(int moduleId, int? pointer, int pageSize, CancellationToken cancellationToken = default);
  Task<ContentSection> CreateContentSectionForModuleAsync(int moduleId, CreateContentSectionCommand command, CancellationToken cancellationToken = default);
  Task<ContentSection?> UpdateContentSectionAsync(int contentSectionId, UpdateContentSectionCommand command, CancellationToken cancellationToken = default);
  Task DeleteContentSectionAsync(int contentSectionId, CancellationToken cancellationToken = default);
}
