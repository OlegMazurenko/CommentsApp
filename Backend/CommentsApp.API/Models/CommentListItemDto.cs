namespace CommentsApp.API.Models;

public record CommentListItemDto
{
    public int Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? HomePage { get; init; }
    public int ReplyCount { get; init; }
    public IEnumerable<UploadedFileDto> Files { get; set; } = [];
}
