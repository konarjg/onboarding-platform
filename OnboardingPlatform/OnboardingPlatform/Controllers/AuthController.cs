namespace OnboardingPlatform.Controllers;

using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dtos;
using System.Security.Claims;
using Application.Dtos;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IUserService userService, IMapper mapper, IWebHostEnvironment environment) : ControllerBase {

  [HttpPost("register")]
  [ProducesResponseType(typeof(UserResponse), 201)]
  [ProducesResponseType(400)]
  public async Task<IActionResult> Register([FromBody] RegisterUserRequest request) {
    User newUser = await userService.RegisterAsync(request.Email, request.Password, UserRole.Employee);
    UserResponse response = mapper.Map<UserResponse>(newUser);
    return CreatedAtAction(nameof(GetMe), new { }, response);
  }

  [HttpPost("login")]
  [ProducesResponseType(typeof(AccessTokenResponse), 200)]
  [ProducesResponseType(401)]
  public async Task<IActionResult> Login([FromBody] LoginRequest request) {
    AuthResult authResult = await authService.LoginAsync(request.Email, request.Password);
    SetRefreshTokenCookie(authResult.RefreshToken);
    return Ok(new AccessTokenResponse(authResult.AccessToken));
  }
  
  [HttpPost("refresh")]
  [ProducesResponseType(typeof(AccessTokenResponse), 200)]
  [ProducesResponseType(400)]
  public async Task<IActionResult> RefreshToken() {
    string? refreshToken = Request.Cookies["refreshToken"];
    if (string.IsNullOrEmpty(refreshToken)) {
      return BadRequest("Refresh token not found.");
    }
    AuthResult authResult = await authService.RefreshTokenAsync(refreshToken);
    SetRefreshTokenCookie(authResult.RefreshToken);
    return Ok(new AccessTokenResponse(authResult.AccessToken));
  }
  
  [Authorize]
  [HttpPost("logout")]
  [ProducesResponseType(204)]
  public async Task<IActionResult> Logout() {
    string? refreshToken = Request.Cookies["refreshToken"];
    if (!string.IsNullOrEmpty(refreshToken)) {
      await authService.LogoutAsync(refreshToken);
    }
    Response.Cookies.Delete("refreshToken");
    return NoContent();
  }

  [Authorize]
  [HttpGet("me")]
  [ProducesResponseType(typeof(UserResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetMe() {
    string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdString, out int userId)) {
      return Unauthorized();
    }
    User? user = await userService.GetUserByIdAsync(userId);
    if (user is null) {
      return NotFound();
    }
    UserResponse response = mapper.Map<UserResponse>(user);
    return Ok(response);
  }

  private void SetRefreshTokenCookie(RefreshToken refreshToken) {
    CookieOptions options = new CookieOptions {
      HttpOnly = true,
      Expires = refreshToken.Expires,
      SameSite = SameSiteMode.Lax,
      Secure = !environment.IsDevelopment()
    };
    Response.Cookies.Append("refreshToken", refreshToken.Token, options);
  }
}