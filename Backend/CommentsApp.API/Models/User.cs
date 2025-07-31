namespace CommentsApp.API.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? HomePage { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
}
