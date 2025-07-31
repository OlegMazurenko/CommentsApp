namespace CommentsApp.API.Models;

public record CommentDto
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? HomePage { get; init; }
    public string Text { get; init; } = string.Empty;
    public int? ParentCommentId { get; init; }
    public Guid CaptchaId { get; init; }
    public string CaptchaCode { get; init; } = string.Empty;
    public ICollection<UploadedFileDto> Files { get; init; } = [];
}
