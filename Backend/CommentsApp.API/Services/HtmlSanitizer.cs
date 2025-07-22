namespace CommentsApp.API.Services;

public class HtmlSanitizer
{
    public string Sanitize(string input)
    {
        var sanitizer = new Ganss.Xss.HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("a");
        sanitizer.AllowedTags.Add("code");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("strong");
        return sanitizer.Sanitize(input);
    }
}
