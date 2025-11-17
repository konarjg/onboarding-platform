namespace Domain;

public class UserPathProgress {
  public required int UserId { get; set; }
  public required int PathId { get; set; }
  public required DateTime EnrollmentDate { get; set; }
  public DateTime? CompletionDate { get; set; }
  
  public bool IsCompleted => CompletionDate is not null;
  
  public User User { get; init; }
  public Path Path { get; init; }
}
