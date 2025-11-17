namespace OnboardingPlatform.Dtos;

public abstract record CreateContentSectionRequest {
  public abstract ContentSectionTypeDto Type { get; }
  public int Order { get; init; }
}
public record CreateMarkdownSectionRequest(string Content) : CreateContentSectionRequest {
  public override ContentSectionTypeDto Type => ContentSectionTypeDto.Markdown;
}
public record CreateImageSectionRequest(string Title, string RelativeUrl, string Alt, string Caption, int Width, int Height) : CreateContentSectionRequest {
  public override ContentSectionTypeDto Type => ContentSectionTypeDto.Image;
}
