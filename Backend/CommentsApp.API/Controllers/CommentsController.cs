using CommentsApp.API.Models;
using CommentsApp.API.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace CommentsApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    private readonly ICommentService _commentService = commentService;

    [HttpPost]
    public async Task<IActionResult> PostComment([FromForm] CommentDto dto)
    {
        try
        {
            var id = await _commentService.CreateCommentAsync(dto);
            return Ok(id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int page = 1, string sort = "date_desc")
    {
        var (comments, isLastPage) = await _commentService.GetCommentsAsync(page, sort);

        return Ok(new { comments, isLastPage });
    }

    [HttpGet("{id}/replies")]
    public async Task<IActionResult> GetReplies(int id)
    {
        var replies = await _commentService.GetRepliesAsync(id);

        if (replies == null)
        {
            return NotFound();
        }

        return Ok(replies);
    }
}
