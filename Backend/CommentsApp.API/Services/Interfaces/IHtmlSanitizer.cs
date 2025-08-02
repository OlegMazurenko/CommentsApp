namespace CommentsApp.API.Services.Interfaces;

public interface IHtmlSanitizer
{
    string Sanitize(string input);
}
