namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos;

public class UserPathProgressRepository(DatabaseContext databaseContext) : IUserPathProgressRepository {
  
  public async Task<UserPathProgress?> GetByUserAndPathAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    return await databaseContext.UserPathProgresses
      .FirstOrDefaultAsync(upp => upp.UserId == userId && upp.PathId == pathId, cancellationToken);
  }

  public async Task<UserPathProgress?> GetByUserAndPathWithDetailsAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    return await databaseContext.UserPathProgresses
      .Include(upp => upp.Path)
      .ThenInclude(p => p.Modules)
      .AsNoTracking()
      .FirstOrDefaultAsync(upp => upp.UserId == userId && upp.PathId == pathId, cancellationToken);
  }

  public async Task<PagedResult<UserPathProgress>> GetForUserAsync(int userId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    IQueryable<UserPathProgress> query = databaseContext.UserPathProgresses
      .Include(upp => upp.Path)
      .Where(upp => upp.UserId == userId)
      .AsNoTracking();

    if (pointer.HasValue) {
      query = query.Where(upp => upp.PathId > pointer.Value);
    }
    
    List<UserPathProgress> items = await query
      .OrderBy(upp => upp.PathId)
      .Take(pageSize + 1)
      .ToListAsync(cancellationToken);

    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.PathId) : null;
    
    return new PagedResult<UserPathProgress>(items, nextPointer, hasNextPage);
  }

  public void Add(UserPathProgress progress) {
    databaseContext.UserPathProgresses.Add(progress);
  }

  public void Delete(UserPathProgress progress) {
    databaseContext.UserPathProgresses.Remove(progress);
  }

  
  public async Task<PagedResult<UserPathProgressWithCounts>> GetForUserWithProgressCountsAsync(int userId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    IQueryable<UserPathProgress> query = databaseContext.UserPathProgresses
      .Where(upp => upp.UserId == userId)
      .AsNoTracking();

    if (pointer.HasValue) {
      query = query.Where(upp => upp.PathId > pointer.Value);
    }
    
    IQueryable<UserPathProgressWithCounts> projectedQuery = query
      .Select(enrollment => new UserPathProgressWithCounts(
        new UserPathProgress {
          UserId = enrollment.UserId,
          PathId = enrollment.PathId,
          EnrollmentDate = enrollment.EnrollmentDate,
          Path = enrollment.Path
        },
        databaseContext.Modules.Count(m => m.PathId == enrollment.PathId),
        databaseContext.UserModuleProgresses.Count(ump =>
          ump.UserId == userId &&
          ump.CompletionDate != null &&
          databaseContext.Modules.Any(m => m.Id == ump.ModuleId && m.PathId == enrollment.PathId)
        )
      ));
      
    List<UserPathProgressWithCounts> items = await projectedQuery
      .OrderBy(p => p.Enrollment.PathId)
      .Take(pageSize + 1)
      .ToListAsync(cancellationToken);

    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.Enrollment.PathId) : null;
    
    return new PagedResult<UserPathProgressWithCounts>(items, nextPointer, hasNextPage);
  }
}