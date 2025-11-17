namespace Domain;

public abstract class ContentSection {
  public int Id { get; init; }
  public required int ModuleId { get; set; }
  public required int Order { get; set; }
  
  public Module Module { get; init; }
}
