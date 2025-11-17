namespace Application.Ports;

using Domain;

public interface ITokenGenerator {
  string GenerateAccessToken(User user);
  RefreshToken GenerateRefreshToken(User user);
}
