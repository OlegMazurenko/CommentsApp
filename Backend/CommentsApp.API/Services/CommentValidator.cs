using CommentsApp.API.Models;
using CommentsApp.API.Services.Interfaces;

using System.Text.RegularExpressions;

namespace CommentsApp.API.Services;

public class CommentValidator : ICommentValidator
{
    public List<string> Validate(CommentDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.UserName) || !IsAlphaNumeric(dto.UserName))
        {
            errors.Add("User Name must contain only Latin letters and digits.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
        {
            errors.Add("Invalid Email format.");
        }

        if (!string.IsNullOrWhiteSpace(dto.HomePage) && !IsValidUrl(dto.HomePage))
        {
            errors.Add("Invalid Home Page URL.");
        }

        if (string.IsNullOrWhiteSpace(dto.CaptchaCode) || !IsAlphaNumeric(dto.CaptchaCode))
        {
            errors.Add("CAPTCHA code must contain only Latin letters and digits.");
        }

        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            errors.Add("Comment text is required.");
        }

        return errors;
    }

    private bool IsAlphaNumeric(string input) => Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
    private bool IsValidEmail(string input) => Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private bool IsValidUrl(string input) => Uri.TryCreate(input, UriKind.Absolute, out _);
}
