namespace Domain.Commands;

public abstract record UpdateContentSectionCommand {
  public abstract ContentSectionType Type { get; }
  public int Order { get; init; }
}

public record UpdateMarkdownSectionCommand(string Content) : UpdateContentSectionCommand {

  public override ContentSectionType Type => ContentSectionType.Markdown;
}

public record UpdateImageSectionCommand(string Title, string RelativeUrl, string Alt, string Caption, int Width, int Height) : UpdateContentSectionCommand {

  public override ContentSectionType Type => ContentSectionType.Image;
}
