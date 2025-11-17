namespace OnboardingPlatform.Controllers;

using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dtos;

[ApiController]
[Route("api")]
[Authorize(Roles = "Manager,HumanResources,Admin")]
public class ContentManagementController(IContentManagementService contentManagementService, IMapper mapper) : ControllerBase {
  
  [HttpGet("paths")]
  [ProducesResponseType(typeof(PagedResponse<PathResponse>), 200)]
  public async Task<IActionResult> GetAllPaths([FromQuery] int? pointer, [FromQuery] int pageSize = 20) {
    PagedResult<Path> pagedPaths = await contentManagementService.GetAllPathsAsync(pointer, pageSize);
    PagedResponse<PathResponse> response = mapper.Map<PagedResponse<PathResponse>>(pagedPaths);
    return Ok(response);
  }

  [HttpGet("paths/{pathId}")]
  [ProducesResponseType(typeof(PathResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetPath(int pathId) {
    Path? path = await contentManagementService.GetPathByIdAsync(pathId);
    if (path is null) {
      return NotFound();
    }
    PathResponse response = mapper.Map<PathResponse>(path);
    return Ok(response);
  }

  [HttpPost("paths")]
  [ProducesResponseType(typeof(PathResponse), 201)]
  public async Task<IActionResult> CreatePath([FromBody] CreatePathRequest request) {
    CreatePathCommand command = mapper.Map<CreatePathCommand>(request);
    Path newPath = await contentManagementService.CreatePathAsync(command);
    PathResponse response = mapper.Map<PathResponse>(newPath);
    return CreatedAtAction(nameof(GetPath), new { pathId = newPath.Id }, response);
  }

  [HttpPut("paths/{pathId}")]
  [ProducesResponseType(typeof(PathResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> UpdatePath(int pathId, [FromBody] UpdatePathRequest request) {
    UpdatePathCommand command = mapper.Map<UpdatePathCommand>(request);
    Path? updatedPath = await contentManagementService.UpdatePathAsync(pathId, command);
    if (updatedPath is null) {
      return NotFound();
    }
    PathResponse response = mapper.Map<PathResponse>(updatedPath);
    return Ok(response);
  }

  [HttpDelete("paths/{pathId}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> DeletePath(int pathId) {
    await contentManagementService.DeletePathAsync(pathId);
    return NoContent();
  }
  
  [HttpGet("paths/{pathId}/modules")]
  [ProducesResponseType(typeof(PagedResponse<ModuleResponse>), 200)]
  public async Task<IActionResult> GetModulesForPath(int pathId, [FromQuery] int? pointer, [FromQuery] int pageSize = 20) {
    PagedResult<Module> pagedModules = await contentManagementService.GetModulesForPathAsync(pathId, pointer, pageSize);
    PagedResponse<ModuleResponse> response = mapper.Map<PagedResponse<ModuleResponse>>(pagedModules);
    return Ok(response);
  }

  [HttpGet("modules/{moduleId}")]
  [ProducesResponseType(typeof(ModuleResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetModule(int moduleId) {
    Module? module = await contentManagementService.GetModuleByIdAsync(moduleId);
    if (module is null) {
      return NotFound();
    }
    ModuleResponse response = mapper.Map<ModuleResponse>(module);
    return Ok(response);
  }

  [HttpPost("paths/{pathId}/modules")]
  [ProducesResponseType(typeof(ModuleResponse), 201)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> CreateModule(int pathId, [FromBody] CreateModuleRequest request) {
    CreateModuleCommand command = mapper.Map<CreateModuleCommand>(request);
    Module newModule = await contentManagementService.CreateModuleForPathAsync(pathId, command);
    ModuleResponse response = mapper.Map<ModuleResponse>(newModule);
    return CreatedAtAction(nameof(GetModule), new { moduleId = newModule.Id }, response);
  }

  [HttpPut("modules/{moduleId}")]
  [ProducesResponseType(typeof(ModuleResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> UpdateModule(int moduleId, [FromBody] UpdateModuleRequest request) {
    UpdateModuleCommand command = mapper.Map<UpdateModuleCommand>(request);
    Module? updatedModule = await contentManagementService.UpdateModuleAsync(moduleId, command);
    if (updatedModule is null) {
      return NotFound();
    }
    ModuleResponse response = mapper.Map<ModuleResponse>(updatedModule);
    return Ok(response);
  }

  [HttpDelete("modules/{moduleId}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> DeleteModule(int moduleId) {
    await contentManagementService.DeleteModuleAsync(moduleId);
    return NoContent();
  }
  
  [HttpGet("modules/{moduleId}/content")]
  [ProducesResponseType(typeof(PagedResponse<ContentSectionResponse>), 200)]
  public async Task<IActionResult> GetContentForModule(int moduleId, [FromQuery] int? pointer, [FromQuery] int pageSize = 20) {
    PagedResult<ContentSection> pagedContent = await contentManagementService.GetContentForModuleAsync(moduleId, pointer, pageSize);
    PagedResponse<ContentSectionResponse> response = mapper.Map<PagedResponse<ContentSectionResponse>>(pagedContent);
    return Ok(response);
  }

  [HttpGet("content/{contentId}")]
  [ProducesResponseType(typeof(ContentSectionResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetContentSection(int contentId) {
    ContentSection? content = await contentManagementService.GetContentSectionByIdAsync(contentId);
    if (content is null) {
      return NotFound();
    }
    ContentSectionResponse response = mapper.Map<ContentSectionResponse>(content);
    return Ok(response);
  }

  [HttpPost("modules/{moduleId}/content")]
  [ProducesResponseType(typeof(ContentSectionResponse), 201)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> CreateContentSection(int moduleId, [FromBody] CreateContentSectionRequest request) {
    CreateContentSectionCommand command = MapToCreateCommand(request);
    ContentSection newContent = await contentManagementService.CreateContentSectionForModuleAsync(moduleId, command);
    ContentSectionResponse response = mapper.Map<ContentSectionResponse>(newContent);
    return CreatedAtAction(nameof(GetContentSection), new { contentId = newContent.Id }, response);
  }

  [HttpPut("content/{contentId}")]
  [ProducesResponseType(typeof(ContentSectionResponse), 200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> UpdateContentSection(int contentId, [FromBody] UpdateContentSectionRequest request) {
    UpdateContentSectionCommand command = MapToUpdateCommand(request);
    ContentSection? updatedContent = await contentManagementService.UpdateContentSectionAsync(contentId, command);
    if (updatedContent is null) {
      return NotFound();
    }
    ContentSectionResponse response = mapper.Map<ContentSectionResponse>(updatedContent);
    return Ok(response);
  }

  [HttpDelete("content/{contentId}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> DeleteContentSection(int contentId) {
    await contentManagementService.DeleteContentSectionAsync(contentId);
    return NoContent();
  }

  private CreateContentSectionCommand MapToCreateCommand(CreateContentSectionRequest request) {
    return request switch {
      CreateMarkdownSectionRequest md => new CreateMarkdownSectionCommand(md.Content) { Order = request.Order },
      CreateImageSectionRequest img => new CreateImageSectionCommand(img.Title, img.RelativeUrl, img.Alt, img.Caption, img.Width, img.Height) { Order = request.Order },
      _ => throw new NotSupportedException("Unsupported content section request type.")
    };
  }

  private UpdateContentSectionCommand MapToUpdateCommand(UpdateContentSectionRequest request) {
    return request switch {
      UpdateMarkdownSectionRequest md => new UpdateMarkdownSectionCommand(md.Content) { Order = request.Order },
      UpdateImageSectionRequest img => new UpdateImageSectionCommand(img.Title, img.RelativeUrl, img.Alt, img.Caption, img.Width, img.Height) { Order = request.Order },
      _ => throw new NotSupportedException("Unsupported content section request type.")
    };
  }
}