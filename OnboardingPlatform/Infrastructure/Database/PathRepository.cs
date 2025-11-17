namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PathRepository(DatabaseContext databaseContext) : IPathRepository {

  public async Task<Path?> GetByIdAsync(int pathId, CancellationToken cancellationToken = default) {
    return await databaseContext.Paths
                                .FirstOrDefaultAsync(p => p.Id == pathId, cancellationToken);
  }

  public async Task<PagedResult<Path>> GetAllAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    IQueryable<Path> query = databaseContext.Paths.AsNoTracking();

    if (pointer.HasValue) {
      query = query.Where(p => p.Id > pointer.Value);
    }
    
    List<Path> items = await query
                             .OrderBy(p => p.Id)
                             .Take(pageSize + 1)
                             .ToListAsync(cancellationToken);

    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.Id) : null;
    
    return new PagedResult<Path>(items, nextPointer, hasNextPage);
  }

  public void Add(Path path) {
    databaseContext.Paths.Add(path);
  }

  public void Delete(Path path) {
    databaseContext.Paths.Remove(path);
  }
}
