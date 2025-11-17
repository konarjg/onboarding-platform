namespace Domain.Commands;

public abstract record CreateContentSectionCommand {
  public abstract ContentSectionType Type { get; }
  public int Order { get; init; }
}

public record CreateMarkdownSectionCommand(string Content) : CreateContentSectionCommand {

  public override ContentSectionType Type => ContentSectionType.Markdown;
}

public record CreateImageSectionCommand(string Title, string RelativeUrl, string Alt, string Caption, int Width, int Height) : CreateContentSectionCommand {

  public override ContentSectionType Type => ContentSectionType.Image;
}
