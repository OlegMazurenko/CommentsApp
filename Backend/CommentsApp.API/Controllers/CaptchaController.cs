using CommentsApp.API.Data;
using CommentsApp.API.Models;
using CommentsApp.API.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace CommentsApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaptchaController(ICaptchaService generator, AppDbContext context) : ControllerBase
{
    private readonly ICaptchaService _generator = generator;
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<IActionResult> Generate()
    {
        var (image, code) = _generator.GenerateCaptcha();
        var id = Guid.NewGuid();

        _context.Captchas.Add(new CaptchaEntry
        {
            Id = id,
            Code = code,
            Expiration = DateTime.UtcNow.AddMinutes(5)
        });

        await _context.SaveChangesAsync();

        return File(image, "image/png", $"{id}.png");
    }
}
