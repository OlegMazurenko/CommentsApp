namespace CommentsApp.API.Models;

public class FileUpload
{
    public int Id { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public byte[] Content { get; set; } = default!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int CommentId { get; set; }
    public Comment Comment { get; set; } = default!;
}
