namespace Domain;

public class User {
  public int Id { get; init; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public required UserRole Role { get; set; }
  public required bool IsActive { get; set; }
  
  public ICollection<UserPathProgress> AssignedPaths { get; init; }
  public ICollection<UserModuleProgress> AssignedModules { get; init; }
  public ICollection<RefreshToken> Sessions { get; init; }
}
