namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos;

public class UserModuleProgressRepository(DatabaseContext databaseContext) : IUserModuleProgressRepository{
  public async Task<UserModuleProgress?> GetByUserAndModuleAsync(int userId, int moduleId, CancellationToken cancellationToken = default) {
    return await databaseContext.UserModuleProgresses
                                .FirstOrDefaultAsync(ump => ump.UserId == userId && ump.ModuleId == moduleId, cancellationToken);
  }

  public async Task<int> GetCompletedCountForUserAndPathAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    return await databaseContext.UserModuleProgresses
                                .CountAsync(ump => ump.UserId == userId 
                                                   && ump.CompletionDate != null 
                                                   && ump.Module.PathId == pathId, 
                                  cancellationToken);
  }

  public async Task<List<ModuleCompletionStatus>> GetStatusesForUserAndPathAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    return await databaseContext.Modules
                                .Where(m => m.PathId == pathId)
                                .AsNoTracking()
                                .Select(m => new ModuleCompletionStatus(
                                  m.Id,
                                  m.Title,
                                  m.AssignedUsers.Any(ump => ump.UserId == userId && ump.CompletionDate != null)
                                ))
                                .ToListAsync(cancellationToken);
  }

  public void Add(UserModuleProgress progress) {
    databaseContext.UserModuleProgresses.Add(progress);
  }
}
