namespace Infrastructure;

using Application.Ports;
using Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenGenerator(IOptions<JwtOptions> jwtOptions) : ITokenGenerator {
  private readonly JwtOptions _jwtOptions = jwtOptions.Value;

  public string GenerateAccessToken(User user) {
    List<Claim> claims = new List<Claim> {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim(ClaimTypes.Role, user.Role.ToString())
    };
    
    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
    SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    DateTime expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

    JwtSecurityToken token = new JwtSecurityToken(
      issuer: _jwtOptions.Issuer,
      audience: _jwtOptions.Audience,
      claims: claims,
      expires: expires,
      signingCredentials: credentials);
      
    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public RefreshToken GenerateRefreshToken(User user) {
    byte[] randomNumber = new byte[64];
    using RandomNumberGenerator rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    string token = Convert.ToBase64String(randomNumber);

    return new RefreshToken {
      UserId = user.Id,
      Token = token,
      Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
      Revoked = null
    };
  }
}
