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
}
