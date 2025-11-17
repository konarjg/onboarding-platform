namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ModuleRepository(DatabaseContext databaseContext) : IModuleRepository {

  public async Task<Module?> GetByIdAsync(int moduleId, CancellationToken cancellationToken = default) {
    return await databaseContext.Modules
                                .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);
  }

  public async Task<PagedResult<Module>> GetForPathAsync(int pathId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    IQueryable<Module> query = databaseContext.Modules
                                              .Where(m => m.PathId == pathId)
                                              .AsNoTracking();

    if (pointer.HasValue) {
      query = query.Where(m => m.Id > pointer.Value);
    }
    
    List<Module> items = await query
                               .OrderBy(m => m.Id)
                               .Take(pageSize + 1)
                               .ToListAsync(cancellationToken);

    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.Id) : null;
    
    return new PagedResult<Module>(items, nextPointer, hasNextPage);
  }

  public void Add(Module module) {
    databaseContext.Modules.Add(module);
  }

  public void Delete(Module module) {
    databaseContext.Modules.Remove(module);
  }

  public async Task<int> GetCountForPathAsync(int pathId, CancellationToken cancellationToken = default) {
    return await databaseContext.Modules
                                .CountAsync(m => m.PathId == pathId, cancellationToken);
  }
}
