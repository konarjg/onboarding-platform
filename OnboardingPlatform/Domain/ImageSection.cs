namespace Domain;

public class ImageSection : ContentSection {
  public required string Title { get; set; }
  public required string RelativeUrl { get; set; }
  public required string Alt { get; set; }
  public required string Caption { get; set; }
  public required int Width { get; set; }
  public required int Height { get; set; }
}
