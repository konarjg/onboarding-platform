namespace Application;

using Domain;
using Exceptions;
using Dtos;
using Interfaces;
using Ports;

public class AuthService(
  IUserRepository userRepository,
  IRefreshTokenRepository refreshTokenRepository,
  IPasswordHasher passwordHasher,
  ITokenGenerator tokenGenerator,
  IUnitOfWork unitOfWork
  ) : IAuthService {

  public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default) {
    User? user = await userRepository.GetByEmailAsync(email, cancellationToken);

    if (user is null || !user.IsActive || !passwordHasher.VerifyPassword(password, user.PasswordHash)) {
      throw new AuthenticationFailedException("Invalid email or password.");
    }

    string accessToken = tokenGenerator.GenerateAccessToken(user);
    RefreshToken refreshToken = tokenGenerator.GenerateRefreshToken(user);

    refreshTokenRepository.Add(refreshToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return new AuthResult(accessToken, refreshToken);
  }

  public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) {
    RefreshToken? existingToken = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

    if (existingToken is null || existingToken.Revoked is not null || existingToken.Expires <= DateTime.UtcNow) {
      throw new InvalidRefreshTokenException("Invalid refresh token.");
    }

    User user = existingToken.User;
    if (!user.IsActive) {
      throw new AuthenticationFailedException("User account is inactive.");
    }

    string newAccessToken = tokenGenerator.GenerateAccessToken(user);
    RefreshToken newRefreshToken = tokenGenerator.GenerateRefreshToken(user);

    existingToken.Revoked = DateTime.UtcNow;
    refreshTokenRepository.Add(newRefreshToken);

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return new AuthResult(newAccessToken, newRefreshToken);
  }

  public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default) {
    RefreshToken? token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
    if (token is not null && token.Revoked is null) {
      token.Revoked = DateTime.UtcNow;
      await unitOfWork.SaveChangesAsync(cancellationToken);
    }
  }
}