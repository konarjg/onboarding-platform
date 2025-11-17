namespace Application.Dtos;

using Domain;

public record AuthResult(string AccessToken, RefreshToken RefreshToken);
