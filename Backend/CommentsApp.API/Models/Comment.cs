namespace CommentsApp.API.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ParentCommentId { get; set; }
    public int UserId { get; set; }

    public Comment? ParentComment { get; set; }
    public User User { get; set; } = default!;

    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<FileUpload> Files { get; set; } = [];
}
