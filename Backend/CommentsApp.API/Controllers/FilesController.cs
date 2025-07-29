using CommentsApp.API.Data;

using Microsoft.AspNetCore.Mvc;

namespace CommentsApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var file = await _context.FileUploads.FindAsync(id);

        return file == null
            ? NotFound()
            : File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("{id}/text")]
    public async Task<IActionResult> GetTextContent(int id)
    {
        var file = await _context.FileUploads.FindAsync(id);

        if (file == null || file.ContentType != "text/plain")
        {
            return NotFound();
        }

        var content = System.Text.Encoding.UTF8.GetString(file.Content);
        return Content(content, "text/plain");
    }
}
