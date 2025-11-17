namespace Domain;

public class Path {
  public int Id { get; init; }
  public required string Title { get; set; }
  public required string SummaryMarkdown { get; set; }
  
  public ICollection<UserPathProgress> AssignedUsers { get; init; }
  public ICollection<Module> Modules { get; init; }
}
