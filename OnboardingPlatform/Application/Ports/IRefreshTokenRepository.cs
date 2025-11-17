namespace Application.Ports;

using Domain;

public interface IRefreshTokenRepository {
  Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
  void Add(RefreshToken token);
}
