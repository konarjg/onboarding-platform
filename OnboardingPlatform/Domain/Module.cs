namespace Domain;

public class Module {
  public int Id { get; init; }
  public required int PathId { get; set; }
  public required string Title { get; set; }
  public required string SummaryMarkdown { get; set; }
  
  public Path Path { get; init; }
  public ICollection<UserModuleProgress> AssignedUsers { get; init; }
  public ICollection<ContentSection> ContentSections { get; init; }
}
