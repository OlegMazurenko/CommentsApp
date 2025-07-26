using CommentsApp.API.Data;
using CommentsApp.API.Models;
using CommentsApp.API.Services;

using Microsoft.AspNetCore.Mvc;

namespace CommentsApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaptchaController : ControllerBase
{
    private readonly CaptchaGenerator _generator;
    private readonly AppDbContext _context;

    public CaptchaController(CaptchaGenerator generator, AppDbContext context)
    {
        _generator = generator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Generate()
    {
        var (image, code) = _generator.GenerateCaptcha();
        var id = Guid.NewGuid();

        _context.Captchas.Add(new CaptchaEntry
        {
            Id = id,
            Code = code,
            Expiration = DateTime.UtcNow.AddDays(30) // for testing purposes
        });

        await _context.SaveChangesAsync();

        return File(image, "image/png", $"{id}.png");
    }
}
