namespace Infrastructure.Database;

using Application.Ports;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class RefreshTokenRepository(DatabaseContext databaseContext) : IRefreshTokenRepository {

  public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) {
    return await databaseContext.RefreshTokens
                                .Include(rt => rt.User)
                                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
  }

  public void Add(RefreshToken token) {
    databaseContext.RefreshTokens.Add(token);
  }
}