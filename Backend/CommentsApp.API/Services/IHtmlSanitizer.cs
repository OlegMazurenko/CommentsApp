namespace CommentsApp.API.Services;

public interface IHtmlSanitizer
{
    string Sanitize(string input);
}
