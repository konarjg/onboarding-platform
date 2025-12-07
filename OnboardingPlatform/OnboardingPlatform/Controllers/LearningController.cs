namespace OnboardingPlatform.Controllers;

using Application.Interfaces;
using Application.Dtos;
using AutoMapper;
using Domain;
using Domain.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dtos;
using System.Security.Claims;

[ApiController]
[Route("api/me")]
[Authorize]
public class LearningController(ILearningService learningService, IMapper mapper) : ControllerBase {

  [HttpGet("paths")]
  [ProducesResponseType(typeof(PagedResponse<UserPathProgressResponse>), 200)]
  public async Task<IActionResult> GetAssignedPaths([FromQuery] int? pointer, [FromQuery] int pageSize = 20) {
    int userId = GetCurrentUserId();
    PagedResult<UserPathWithProgress> pagedProgress = await learningService.GetUserAssignedPathsAsync(userId, pointer, pageSize);
    PagedResponse<UserPathProgressResponse> response = mapper.Map<PagedResponse<UserPathProgressResponse>>(pagedProgress);
    return Ok(response);
  }

  [HttpGet("paths/{pathId}")]
  [ProducesResponseType(typeof(UserPathDetailsResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetAssignedPathDetails(int pathId) {
    int userId = GetCurrentUserId();
    UserPathDetails? pathDetails = await learningService.GetUserAssignedPathDetailsAsync(userId, pathId);
    if (pathDetails is null) {
      return NotFound();
    }
    UserPathDetailsResponse response = mapper.Map<UserPathDetailsResponse>(pathDetails);
    return Ok(response);
  }

  [HttpGet("modules/{moduleId}/progress")]
  [ProducesResponseType(typeof(ModuleProgressResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetModuleProgress(int moduleId) {
    int userId = GetCurrentUserId();
    UserModuleProgress? progress = await learningService.GetModuleProgressForUserAsync(userId, moduleId);
    if (progress is null) {
      return NotFound();
    }
    ModuleProgressResponse response = mapper.Map<ModuleProgressResponse>(progress);
    return Ok(response);
  }

  [HttpPut("modules/{moduleId}/progress")]
  [ProducesResponseType(204)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> UpdateModuleProgress(int moduleId, [FromBody] UpdateModuleProgressRequest request) {
    int userId = GetCurrentUserId();
    UpdateModuleProgressCommand command = mapper.Map<UpdateModuleProgressCommand>(request);
    await learningService.UpdateModuleProgressAsync(userId, moduleId, command);
    return NoContent();
  }
  
  [HttpGet("modules/{moduleId}/content")]
  [ProducesResponseType(typeof(PagedResponse<ContentSectionResponse>), 200)]
  public async Task<IActionResult> GetModuleContent(int moduleId, [FromQuery] int? pointer, [FromQuery] int pageSize) {
    PagedResult<ContentSection> content = await learningService.GetModuleContentAsync(moduleId, pointer, pageSize);
    PagedResponse<ContentSectionResponse> response = mapper.Map<PagedResponse<ContentSectionResponse>>(content);
    return Ok(response);
  }
  
  private int GetCurrentUserId() {
    string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (int.TryParse(userIdString, out int userId)) {
      return userId;
    }
    throw new InvalidOperationException("User ID could not be parsed from token claims.");
  }
}