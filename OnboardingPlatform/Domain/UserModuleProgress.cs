namespace Domain;

public class UserModuleProgress {
  public required int UserId { get; set; }
  public required int ModuleId { get; set; }
  public DateTime? CompletionDate { get; set; }
  
  public bool IsCompleted => CompletionDate is not null;
  
  public User User { get; set; }
  public Module Module { get; set; }
}
