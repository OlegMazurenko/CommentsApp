using CommentsApp.API.Data;
using CommentsApp.API.Services.Interfaces;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.API.Services;

public class CaptchaService(AppDbContext context) : ICaptchaService
{
    private readonly AppDbContext _context = context;

    public (byte[] ImageBytes, string Code) GenerateCaptcha()
    {
        var fontCollection = new FontCollection();
        fontCollection.AddSystemFonts();

        var fontFamily = fontCollection.TryGet("Arial", out var arialFamily)
            ? arialFamily
            : fontCollection.Families.First();

        var font = fontFamily.CreateFont(24, FontStyle.Bold);

        var code = Guid.NewGuid().ToString("N")[..6].ToUpper();
        using var image = new Image<Rgba32>(120, 40);

        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);
            ctx.DrawText(code, font, Color.Black, new PointF(10, 5));
        });

        using var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);

        return (memoryStream.ToArray(), code);
    }

    public async Task<bool> ValidateCaptchaAsync(Guid id, string code)
    {
        var captcha = await _context.Captchas.FindAsync(id);

        return captcha != null && captcha.Code == code && captcha.Expiration >= DateTime.UtcNow;
    }
}
