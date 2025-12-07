namespace Application;

using Domain;
using Domain.Commands;
using Exceptions;
using Interfaces;
using Ports;
using System.Collections.Generic;
using System.Linq;
using Dtos;

public class LearningService(
    IPathRepository pathRepository, 
    IModuleRepository moduleRepository, 
    IUserRepository userRepository, 
    IUserPathProgressRepository userPathProgressRepository,
    IUserModuleProgressRepository userModuleProgressRepository,
    IContentSectionRepository contentSectionRepository,
    IUnitOfWork unitOfWork) : ILearningService {

  public async Task<UserPathProgress> EnrollUserInPathAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    if (await userRepository.GetByIdAsync(userId, cancellationToken) is null) {
      throw new UserNotFoundException($"User with id: {userId} does not exist!");
    }
    
    if (await pathRepository.GetByIdAsync(pathId, cancellationToken) is null) {
      throw new PathNotFoundException($"Path with id: {pathId} does not exist!");
    }
    
    if (await userPathProgressRepository.GetByUserAndPathAsync(userId, pathId, cancellationToken) is not null) {
      throw new UserAlreadyEnrolledException($"User with id: {userId} is already enrolled in path with id: {pathId}.");
    }

    UserPathProgress enrollment = new UserPathProgress {
      UserId = userId,
      PathId = pathId,
      EnrollmentDate = DateTime.UtcNow
    };
    
    userPathProgressRepository.Add(enrollment);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return enrollment;
  }
  
  public async Task UnenrollUserFromPathAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    UserPathProgress? enrollment = await userPathProgressRepository.GetByUserAndPathAsync(userId, pathId, cancellationToken);
    if (enrollment is null) {
      throw new UserNotEnrolledException($"User with id: {userId} is not enrolled in path with id: {pathId}.");
    }
    
    userPathProgressRepository.Delete(enrollment);
    await unitOfWork.SaveChangesAsync(cancellationToken);
  }
  
  public async Task<PagedResult<UserPathWithProgress>> GetUserAssignedPathsAsync(int userId, int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    if (await userRepository.GetByIdAsync(userId, cancellationToken) is null) {
      throw new UserNotFoundException($"User with id: {userId} does not exist!");
    }
    
    PagedResult<UserPathProgressWithCounts> pagedProgressWithCounts = await userPathProgressRepository.GetForUserWithProgressCountsAsync(userId, pointer, pageSize, cancellationToken);
    
    List<UserPathWithProgress> results = pagedProgressWithCounts.Items.Select(item => {
      double progressPercentage = 0;
      if (item.TotalModuleCount > 0) {
        progressPercentage = ((double)item.CompletedModuleCount / item.TotalModuleCount) * 100.0;
      }
      return new UserPathWithProgress(item.Enrollment.Path, progressPercentage);
    }).ToList();
    
    return new PagedResult<UserPathWithProgress>(results, pagedProgressWithCounts.NextPointer, pagedProgressWithCounts.HasNextPage);
  }

  public async Task<UserPathDetails?> GetUserAssignedPathDetailsAsync(int userId, int pathId, CancellationToken cancellationToken = default) {
    UserPathProgress? enrollment = await userPathProgressRepository.GetByUserAndPathWithDetailsAsync(userId, pathId, cancellationToken);
    if (enrollment is null) {
      return null;
    }

    List<ModuleCompletionStatus> moduleStatuses = await userModuleProgressRepository.GetStatusesForUserAndPathAsync(userId, pathId, cancellationToken);
    
    UserPathDetails result = new UserPathDetails(enrollment.Path, enrollment, moduleStatuses);
    
    return result;
  }
  
  public async Task<UserModuleProgress?> UpdateModuleProgressAsync(int userId, int moduleId, UpdateModuleProgressCommand command, CancellationToken cancellationToken = default) {
    UserModuleProgress? progress = await userModuleProgressRepository.GetByUserAndModuleAsync(userId, moduleId, cancellationToken);

    if (progress is null) {
      Module? module = await moduleRepository.GetByIdAsync(moduleId, cancellationToken);
      if (module is null) {
        throw new ModuleNotFoundException($"Module with id: {moduleId} does not exist!");
      }
      
      if (await userPathProgressRepository.GetByUserAndPathAsync(userId, module.PathId, cancellationToken) is null) {
        throw new UserNotEnrolledException($"User with id: {userId} is not enrolled in the parent path and cannot update module progress.");
      }
      
      progress = new UserModuleProgress { UserId = userId, ModuleId = moduleId };
      userModuleProgressRepository.Add(progress);
    }

    if (command.IsCompleted) {
      progress.CompletionDate ??= DateTime.UtcNow;
    } else {
      progress.CompletionDate = null;
    }

    await unitOfWork.SaveChangesAsync(cancellationToken);
    return progress;
  }
  
  public async Task<UserModuleProgress?> GetModuleProgressForUserAsync(int userId, int moduleId, CancellationToken cancellationToken = default) {
    return await userModuleProgressRepository.GetByUserAndModuleAsync(userId, moduleId, cancellationToken);
  }
  public async Task<PagedResult<ContentSection>> GetModuleContentAsync(int moduleId, int? pointer, int pageSize,
    CancellationToken cancellationToken = default) {
    
    return await contentSectionRepository.GetForModuleAsync(moduleId, pointer, pageSize, cancellationToken);
  }
}