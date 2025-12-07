namespace OnboardingPlatform.Controllers;

using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dtos;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Manager,HumanResources,Admin")]
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase {

  [HttpGet]
  [ProducesResponseType(typeof(PagedResponse<UserResponse>), 200)]
  public async Task<IActionResult> GetAllUsers([FromQuery] int? pointer, [FromQuery] int pageSize = 20) {
    PagedResult<User> pagedUsers = await userService.GetAllUsersAsync(pointer, pageSize);
    PagedResponse<UserResponse> response = mapper.Map<PagedResponse<UserResponse>>(pagedUsers);
    return Ok(response);
  }

  [HttpGet("{userId}")]
  [ProducesResponseType(typeof(UserResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetUserById(int userId) {
    User? user = await userService.GetUserByIdAsync(userId);
    if (user is null) {
      return NotFound();
    }
    UserResponse response = mapper.Map<UserResponse>(user);
    return Ok(response);
  }
  
  [HttpPatch("{userId}/role")]
  [ProducesResponseType(typeof(UserResponse), 200)]
  [ProducesResponseType(404)]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRoleRequest request) {
    User? updatedUser = await userService.UpdateUserRoleAsync(userId, request.Role);
    if (updatedUser is null) {
      return NotFound();
    }
    UserResponse response = mapper.Map<UserResponse>(updatedUser);
    return Ok(response);
  }

  [HttpDelete("{userId}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(404)]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> DeactivateUser(int userId) {
    await userService.DeactivateUserAsync(userId);
    return NoContent();
  }
}
