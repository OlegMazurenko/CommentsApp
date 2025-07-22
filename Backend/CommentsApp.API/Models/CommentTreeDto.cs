namespace CommentsApp.API.Models;

public record CommentTreeDto
{
    public int Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? HomePage { get; init; }
    public List<FileMetaDto> Files { get; init; } = new();
    public List<CommentTreeDto> Replies { get; init; } = new();
}
