namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ContentSectionRepository(DatabaseContext databaseContext) : IContentSectionRepository {

  public async Task<ContentSection?> GetByIdAsync(int contentSectionId, CancellationToken cancellationToken = default) {
    return await databaseContext.ContentSections
                                .FirstOrDefaultAsync(cs => cs.Id == contentSectionId, cancellationToken);
  }

  public async Task<PagedResult<ContentSection>> GetForModuleAsync(int moduleId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    IQueryable<ContentSection> query = databaseContext.ContentSections
                                                      .Where(cs => cs.ModuleId == moduleId)
                                                      .AsNoTracking();

    if (pointer.HasValue) {
      query = query.Where(cs => cs.Id > pointer.Value);
    }
    
    List<ContentSection> items = await query
                                       .OrderBy(cs => cs.Order) 
                                       .Take(pageSize + 1)
                                       .ToListAsync(cancellationToken);

    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.Id) : null;
    
    return new PagedResult<ContentSection>(items, nextPointer, hasNextPage);
  }

  public void Add(ContentSection contentSection) {
    databaseContext.ContentSections.Add(contentSection);
  }

  public void Delete(ContentSection contentSection) {
    databaseContext.ContentSections.Remove(contentSection);
  }
}
