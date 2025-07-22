namespace CommentsApp.API.Models;

public class CaptchaEntry
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public DateTime Expiration { get; set; }
}
