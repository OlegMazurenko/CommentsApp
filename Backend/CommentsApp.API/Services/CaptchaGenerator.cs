using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

namespace CommentsApp.API.Services;

public class CaptchaGenerator : ICaptchaGenerator
{
    private readonly Font _font;

    public CaptchaGenerator()
    {
        var fontCollection = new FontCollection();
        fontCollection.AddSystemFonts();

        var fontFamily = fontCollection.TryGet("Arial", out var arialFamily)
            ? arialFamily
            : fontCollection.Families.First();

        _font = fontFamily.CreateFont(24, FontStyle.Bold);
    }

    public (byte[] ImageBytes, string Code) GenerateCaptcha()
    {
        var code = Guid.NewGuid().ToString("N")[..6].ToUpper();
        using var image = new Image<Rgba32>(120, 40);

        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);
            ctx.DrawText(code, _font, Color.Black, new PointF(10, 5));
        });

        using var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);
        return (memoryStream.ToArray(), code);
    }
}
