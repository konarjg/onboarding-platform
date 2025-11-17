namespace OnboardingPlatform.Dtos;

public abstract record UpdateContentSectionRequest {
  public abstract ContentSectionTypeDto Type { get; }
  public int Order { get; init; }
}
public record UpdateMarkdownSectionRequest(string Content) : UpdateContentSectionRequest {
  public override ContentSectionTypeDto Type => ContentSectionTypeDto.Markdown;
}
public record UpdateImageSectionRequest(string Title, string RelativeUrl, string Alt, string Caption, int Width, int Height) : UpdateContentSectionRequest {
  public override ContentSectionTypeDto Type => ContentSectionTypeDto.Image;
}
