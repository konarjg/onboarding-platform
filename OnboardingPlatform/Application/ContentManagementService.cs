namespace Application;

using Domain;
using Domain.Commands;
using Exceptions;
using Interfaces;
using Ports;

public class ContentManagementService(IPathRepository pathRepository, IModuleRepository moduleRepository, IContentSectionRepository contentSectionRepository, IUnitOfWork unitOfWork) : IContentManagementService {

  public async Task<Path?> GetPathByIdAsync(int pathId, CancellationToken cancellationToken = default) {
    return await pathRepository.GetByIdAsync(pathId, cancellationToken);
  }

  public async Task<Path?> UpdatePathAsync(int pathId, UpdatePathCommand command, CancellationToken cancellationToken = default) {
    Path? path = await pathRepository.GetByIdAsync(pathId, cancellationToken);
    if (path is null) {
      throw new PathNotFoundException($"Path with id: {pathId} does not exist!");
    }

    path.Title = command.Title;
    path.SummaryMarkdown = command.SummaryMarkdown;
    
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return path;
  }

  public async Task<PagedResult<Path>> GetAllPathsAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    return await pathRepository.GetAllAsync(pointer, pageSize, cancellationToken);
  }

  public async Task<Path> CreatePathAsync(CreatePathCommand command, CancellationToken cancellationToken = default) {
    Path path = new Path() {
      Title = command.Title,
      SummaryMarkdown = command.SummaryMarkdown,
    };
    
    pathRepository.Add(path);
    await unitOfWork.SaveChangesAsync(cancellationToken);
    
    return path;
  }
  
  public async Task DeletePathAsync(int pathId, CancellationToken cancellationToken = default) {
    Path? pathToDelete = await pathRepository.GetByIdAsync(pathId, cancellationToken);
    if (pathToDelete is null) {
      throw new PathNotFoundException($"Path with id: {pathId} does not exist!");
    }
    
    pathRepository.Delete(pathToDelete);
    await unitOfWork.SaveChangesAsync(cancellationToken);
  }

  public async Task<PagedResult<Module>> GetModulesForPathAsync(int pathId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    if (await pathRepository.GetByIdAsync(pathId, cancellationToken) is null) {
      throw new PathNotFoundException($"Path with id: {pathId} does not exist!");
    }
    return await moduleRepository.GetForPathAsync(pathId, pointer, pageSize, cancellationToken);
  }

  public async Task<Module> CreateModuleForPathAsync(int pathId, CreateModuleCommand command, CancellationToken cancellationToken = default) {
    if (await pathRepository.GetByIdAsync(pathId, cancellationToken) is null) {
      throw new PathNotFoundException($"Cannot create module for a non-existent path with id: {pathId}!");
    }

    Module module = new Module() {
      PathId = pathId,
      Title = command.Title,
      SummaryMarkdown = command.SummaryMarkdown
    };

    moduleRepository.Add(module);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return module;
  }

  public async Task<Module?> GetModuleByIdAsync(int moduleId, CancellationToken cancellationToken = default) {
    return await moduleRepository.GetByIdAsync(moduleId, cancellationToken);
  }

  public async Task<Module?> UpdateModuleAsync(int moduleId, UpdateModuleCommand command, CancellationToken cancellationToken = default) {
    Module? moduleToUpdate = await moduleRepository.GetByIdAsync(moduleId, cancellationToken);
    if (moduleToUpdate is null) {
      throw new ModuleNotFoundException($"Module with id: {moduleId} does not exist!");
    }

    moduleToUpdate.Title = command.Title;
    moduleToUpdate.SummaryMarkdown = command.SummaryMarkdown;
    
    await unitOfWork.SaveChangesAsync(cancellationToken);
    
    return moduleToUpdate;
  }

  public async Task DeleteModuleAsync(int moduleId, CancellationToken cancellationToken = default) {
    Module? moduleToDelete = await moduleRepository.GetByIdAsync(moduleId, cancellationToken);
    if (moduleToDelete is null) {
      throw new ModuleNotFoundException($"Module with id: {moduleId} does not exist!");
    }
    
    moduleRepository.Delete(moduleToDelete);
    await unitOfWork.SaveChangesAsync(cancellationToken);
  }

  public async Task<ContentSection?> GetContentSectionByIdAsync(int contentSectionId, CancellationToken cancellationToken = default) {
    return await contentSectionRepository.GetByIdAsync(contentSectionId, cancellationToken);
  }

  public async Task<PagedResult<ContentSection>> GetContentForModuleAsync(int moduleId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    if (await moduleRepository.GetByIdAsync(moduleId, cancellationToken) is null) {
      throw new ModuleNotFoundException($"Module with id: {moduleId} does not exist!");
    }
    return await contentSectionRepository.GetForModuleAsync(moduleId, pointer, pageSize, cancellationToken);
  }

  public async Task<ContentSection> CreateContentSectionForModuleAsync(int moduleId, CreateContentSectionCommand command, CancellationToken cancellationToken = default) {
    if (await moduleRepository.GetByIdAsync(moduleId, cancellationToken) is null) {
      throw new ModuleNotFoundException($"Cannot create content section for a non-existent module with id: {moduleId}!");
    }
    
    ContentSection newContentSection = command switch {
      CreateMarkdownSectionCommand cmd => new MarkdownSection() {
        ModuleId = moduleId, Order = cmd.Order, Content = cmd.Content
      },
      CreateImageSectionCommand cmd => new ImageSection() {
        ModuleId = moduleId, Order = cmd.Order, Title = cmd.Title, RelativeUrl = cmd.RelativeUrl, Alt = cmd.Alt, Caption = cmd.Caption, Width = cmd.Width, Height = cmd.Height
      },
      _ => throw new NotSupportedException("The provided content section type is not supported.")
    };
    
    contentSectionRepository.Add(newContentSection);
    await unitOfWork.SaveChangesAsync(cancellationToken);
    
    return newContentSection;
  }

  public async Task<ContentSection?> UpdateContentSectionAsync(int contentSectionId, UpdateContentSectionCommand command, CancellationToken cancellationToken = default) {
    ContentSection? existingSection = await contentSectionRepository.GetByIdAsync(contentSectionId, cancellationToken);
    if (existingSection is null) {
      throw new ContentSectionNotFoundException($"ContentSection with id: {contentSectionId} does not exist!");
    }

    switch (command) {
      case UpdateMarkdownSectionCommand cmd when existingSection is MarkdownSection markdownSection:
        markdownSection.Order = cmd.Order;
        markdownSection.Content = cmd.Content;
        break;
      
      case UpdateImageSectionCommand cmd when existingSection is ImageSection imageSection:
        imageSection.Order = cmd.Order;
        imageSection.Title = cmd.Title;
        imageSection.RelativeUrl = cmd.RelativeUrl;
        imageSection.Alt = cmd.Alt;
        imageSection.Caption = cmd.Caption;
        imageSection.Width = cmd.Width;
        imageSection.Height = cmd.Height;
        break;
        
      default:
        throw new InvalidOperationException("Cannot change the type of a content section or the provided update command is invalid for this section type.");
    }

    await unitOfWork.SaveChangesAsync(cancellationToken);
    return existingSection;
  }

  public async Task DeleteContentSectionAsync(int contentSectionId, CancellationToken cancellationToken = default) {
    ContentSection? sectionToDelete = await contentSectionRepository.GetByIdAsync(contentSectionId, cancellationToken);
    if (sectionToDelete is null) {
      throw new ContentSectionNotFoundException($"ContentSection with id: {contentSectionId} does not exist!");
    }
    
    contentSectionRepository.Delete(sectionToDelete);
    await unitOfWork.SaveChangesAsync(cancellationToken);
  }
}