namespace CommentsApp.API.Services.Interfaces;

public interface ICaptchaService
{
    (byte[] ImageBytes, string Code) GenerateCaptcha();
    Task<bool> ValidateCaptchaAsync(Guid id, string code);
}
