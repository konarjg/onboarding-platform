namespace OnboardingPlatform;

using Application.Dtos;
using AutoMapper;
using Domain;
using Domain.Commands;
using Dtos;
using System.Linq;

public class MappingProfile : Profile {
  public MappingProfile() {
    CreateMap<User, UserResponse>();
    
    CreateMap<CreatePathRequest, CreatePathCommand>();
    CreateMap<UpdatePathRequest, UpdatePathCommand>();
    CreateMap<Path, PathResponse>();

    CreateMap<CreateModuleRequest, CreateModuleCommand>();
    CreateMap<UpdateModuleRequest, UpdateModuleCommand>();
    CreateMap<Module, ModuleResponse>();

    CreateMap<ContentSection, ContentSectionResponse>()
      .Include<MarkdownSection, MarkdownSectionResponse>()
      .Include<ImageSection, ImageSectionResponse>();
    
    CreateMap<MarkdownSection, MarkdownSectionResponse>();
    CreateMap<ImageSection, ImageSectionResponse>();
    
    CreateMap<UpdateModuleProgressRequest, UpdateModuleProgressCommand>();

    CreateMap<UserPathWithProgress, UserPathProgressResponse>()
      .ConstructUsing(src => new UserPathProgressResponse(
        src.Path.Id,
        src.Path.Title,
        src.ProgressPercentage
      ));

    CreateMap<ModuleCompletionStatus, ModuleProgressResponse>();

    CreateMap<UserPathDetails, UserPathDetailsResponse>()
      .ConstructUsing(src => new UserPathDetailsResponse(
        src.Path.Id,
        src.Path.Title,
        src.Path.SummaryMarkdown,
        src.EnrollmentDetails.EnrollmentDate,
        CalculateOverallProgress(src.ModuleStatuses),
        src.ModuleStatuses.Select(m => new ModuleProgressResponse(m.ModuleId, m.Title, m.IsCompleted)).ToList()
      ));
    
    CreateMap(typeof(PagedResult<>), typeof(PagedResponse<>));
  }

  private double CalculateOverallProgress(ICollection<ModuleCompletionStatus> statuses) {
    if (statuses is null || !statuses.Any()) {
      return 0;
    }
    
    int completedCount = statuses.Count(s => s.IsCompleted);
    return ((double)completedCount / statuses.Count) * 100.0;
  }
}