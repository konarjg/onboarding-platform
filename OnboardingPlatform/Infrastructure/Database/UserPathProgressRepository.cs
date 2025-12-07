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
    
    // 1. Build Query
    var query = databaseContext.UserPathProgresses
      .AsNoTracking()
      .Where(upp => upp.UserId == userId);

    if (pointer.HasValue) {
      query = query.Where(upp => upp.PathId > pointer.Value);
    }
    
    // 2. Fetch Data (Projection)
    var rawData = await query
      .OrderBy(upp => upp.PathId)
      .Take(pageSize + 1)
      .Select(upp => new {
          // Fetch the raw entity
          Enrollment = upp,
          // Explicitly fetch the navigation property
          Path = upp.Path,        
          
          // Subqueries for counts
          TotalCount = databaseContext.Modules
              .Count(m => m.PathId == upp.PathId),
              
          CompletedCount = databaseContext.UserModuleProgresses
              .Count(ump =>
                  ump.UserId == userId &&
                  ump.CompletionDate != null &&
                  ump.Module.PathId == upp.PathId
              )
      })
      .ToListAsync(cancellationToken);

    // 3. In-Memory Mapping (Fixing the Null Reference)
    List<UserPathProgressWithCounts> items = rawData
      .Select(item => {
          // Since 'Path' is likely an 'init' property, we must create a new object 
          // to assign it. We copy the values from the fetched 'item.Enrollment'.
          var enrollmentWithPath = new UserPathProgress {
              UserId = item.Enrollment.UserId,
              PathId = item.Enrollment.PathId,
              EnrollmentDate = item.Enrollment.EnrollmentDate,
              CompletionDate = item.Enrollment.CompletionDate,
              // LINK THE PATH HERE
              Path = item.Path 
          };
          
          return new UserPathProgressWithCounts(
              enrollmentWithPath,
              item.TotalCount,
              item.CompletedCount
          );
      })
      .ToList();
    
    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.Enrollment.PathId) : null;
    
    return new PagedResult<UserPathProgressWithCounts>(items, nextPointer, hasNextPage);
  }
}