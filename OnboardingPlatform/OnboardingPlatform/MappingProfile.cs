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
      .ForMember(dest => dest.PathId, opt => opt.MapFrom(src => src.Path.Id))
      .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Path.Title))
      .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => src.ProgressPercentage));

    CreateMap<ModuleCompletionStatus, ModuleProgressResponse>();

    CreateMap<UserPathDetails, UserPathDetailsResponse>()
      .ForMember(dest => dest.PathId, opt => opt.MapFrom(src => src.Path.Id))
      .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Path.Title))
      .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Path.SummaryMarkdown))
      .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => src.EnrollmentDetails.EnrollmentDate))
      .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.ModuleStatuses))
      .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => CalculateOverallProgress(src.ModuleStatuses)));
    
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