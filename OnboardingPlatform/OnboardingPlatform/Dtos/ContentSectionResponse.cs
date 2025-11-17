namespace OnboardingPlatform.Dtos;

public abstract record ContentSectionResponse {
  public int Id { get; init; }
  public int ModuleId { get; init; }
  public int Order { get; init; }
  public abstract ContentSectionTypeDto Type { get; }
}

public record MarkdownSectionResponse : ContentSectionResponse {
  public string Content { get; init; }
  public override ContentSectionTypeDto Type => ContentSectionTypeDto.Markdown;
}

public record ImageSectionResponse : ContentSectionResponse {
  public string Title { get; init; }
  public string RelativeUrl { get; init; }
  public string Alt { get; init; }
  public string Caption { get; init; }
  public int Width { get; init; }
  public int Height { get; init; }
  public override ContentSectionTypeDto Type => ContentSectionTypeDto.Image;
}
