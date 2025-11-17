namespace Application.Interfaces;

using Dtos;

public interface IAuthService {
  Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
  Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
  Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}
