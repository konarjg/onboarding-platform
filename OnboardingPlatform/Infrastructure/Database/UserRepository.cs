namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;

public class UserRepository(DatabaseContext databaseContext) : IUserRepository {
  public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default) {
    return await databaseContext.Users
                                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
  }

  public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {
    return await databaseContext.Users
                                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
  }

  public async Task<PagedResult<User>> GetAllAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    IQueryable<User> query = databaseContext.Users.AsNoTracking();

    if (pointer.HasValue) {
      query = query.Where(u => u.Id > pointer.Value);
    }
    
    List<User> items = await query
                             .OrderBy(u => u.Id)
                             .Take(pageSize + 1)
                             .ToListAsync(cancellationToken);

    bool hasNextPage = items.Count > pageSize;
    
    if (hasNextPage) {
      items.RemoveAt(items.Count - 1);
    }
      
    int? nextPointer = items.Any() ? items.Max(i => i.Id) : null;
    
    return new PagedResult<User>(items, nextPointer, hasNextPage);
  }

  public void Add(User user) {
    databaseContext.Users.Add(user);
  }
}
