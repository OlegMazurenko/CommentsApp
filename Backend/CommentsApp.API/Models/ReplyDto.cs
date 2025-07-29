namespace CommentsApp.API.Models;

public record ReplyDto
{
    public int Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? HomePage { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public int RepliesCount { get; init; }
    public List<UploadedFileDto> Files { get; set; } = [];
}
