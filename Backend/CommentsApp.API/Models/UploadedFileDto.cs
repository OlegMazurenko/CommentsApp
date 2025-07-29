namespace CommentsApp.API.Models;

public record UploadedFileDto
{
    public int Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string Base64Content { get; init; } = string.Empty;
}
