namespace CommentsApp.API.Models;

public record FileMetaDto
{
    public int Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
