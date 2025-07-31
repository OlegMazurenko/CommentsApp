namespace CommentsApp.API.Services;

public interface ICaptchaGenerator
{
    (byte[] ImageBytes, string Code) GenerateCaptcha();
}
