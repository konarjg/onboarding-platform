namespace Infrastructure.Database;

using Application.Ports;

public class UnitOfWork(DatabaseContext databaseContext) : IUnitOfWork {

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await databaseContext.SaveChangesAsync(cancellationToken);
  }
}
