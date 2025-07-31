using CommentsApp.API.Data;
using CommentsApp.API.Hubs;
using CommentsApp.API.Models;
using CommentsApp.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(AppDbContext context,
    IHtmlSanitizer sanitizer,
    IHubContext<CommentsHub> hubContext,
    IMemoryCache cache,
    FileProcessorQueue fileProcessorQueue)
    : ControllerBase
{
    private static readonly IList<string> _commentCacheKeys = [];

    private readonly AppDbContext _context = context;
    private readonly IHtmlSanitizer _sanitizer = sanitizer;
    private readonly IHubContext<CommentsHub> _hubContext = hubContext;
    private readonly IMemoryCache _cache = cache;
    private readonly FileProcessorQueue _fileProcessorQueue = fileProcessorQueue;

    [HttpPost]
    public async Task<IActionResult> PostComment([FromForm] CommentDto dto)
    {
        var captcha = await _context.Captchas.FindAsync(dto.CaptchaId);

        if (captcha == null || captcha.Code != dto.CaptchaCode || captcha.Expiration < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired CAPTCHA");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                HomePage = dto.HomePage
            };

            _context.Users.Add(user);
        }

        var parentComment = dto.ParentCommentId.HasValue
            ? await _context.Comments.FindAsync(dto.ParentCommentId.Value)
            : null;

        if (dto.ParentCommentId.HasValue && parentComment == null)
        {
            return BadRequest($"Parent comment with Id {dto.ParentCommentId} not found.");
        }

        var comment = new Comment
        {
            Text = _sanitizer.Sanitize(dto.Text),
            User = user,
            ParentCommentId = dto.ParentCommentId
        };

        foreach (var fileDto in dto.Files)
        {
            byte[] content;

            try
            {
                content = Convert.FromBase64String(fileDto.Base64Content);
            }
            catch
            {
                return BadRequest($"Invalid base64 in file: {fileDto.FileName}");
            }

            if (fileDto.ContentType == "text/plain" && content.Length > 100_000)
                return BadRequest("Text file too large");

            if (fileDto.ContentType.StartsWith("image/"))
            {
                content = ResizeImageIfNeeded(content, 320, 240);
            }

            var file = new FileUpload
            {
                FileName = fileDto.FileName,
                ContentType = fileDto.ContentType,
                Content = content,
                Comment = comment
            };

            _context.FileUploads.Add(file);
        }

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        _fileProcessorQueue.Enqueue(comment);

        lock (_commentCacheKeys)
        {
            foreach (var key in _commentCacheKeys)
            {
                _cache.Remove(key);
            }
            _commentCacheKeys.Clear();
        }

        await _hubContext.Clients.All.SendAsync("NewComment", comment.Id);

        return Ok(comment.Id);
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int page = 1, string sort = "date_desc")
    {
        const int pageSize = 25;
        var cacheKey = $"comments:page:{page}:sort:{sort}";

        if (_cache.TryGetValue(cacheKey, out object? cached) && cached is not null)
        {
            return Ok(cached);
        }

        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ParentCommentId == null);

        query = sort switch
        {
            "user_asc" => query.OrderBy(c => c.User.UserName),
            "user_desc" => query.OrderByDescending(c => c.User.UserName),
            "email_asc" => query.OrderBy(c => c.User.Email),
            "email_desc" => query.OrderByDescending(c => c.User.Email),
            "date_asc" => query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt),
        };

        var commentEntities = await query
            .Include(c => c.Replies)
            .Include(c => c.Files)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .ToListAsync();

        var isLastPage = commentEntities.Count <= pageSize;

        var comments = commentEntities
            .Take(pageSize)
            .ToList();

        var result = comments.Select(c => new CommentListItemDto
        {
            Id = c.Id,
            Text = c.Text,
            CreatedAt = c.CreatedAt,
            UserName = c.User.UserName,
            Email = c.User.Email,
            HomePage = c.User.HomePage,
            ReplyCount = c.Replies.Count,
            Files = c.Files.Select(f => new UploadedFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType
            }).ToList()
        }).ToList();

        var response = new
        {
            comments = result,
            isLastPage
        };

        _cache.Set(cacheKey, response, TimeSpan.FromSeconds(60));

        lock (_commentCacheKeys)
        {
            if (!_commentCacheKeys.Contains(cacheKey))
            {
                _commentCacheKeys.Add(cacheKey);
            }
        }

        return Ok(response);
    }

    [HttpGet("{id}/replies")]
    public async Task<IActionResult> GetReplies(int id)
    {
        var parent = await _context.Comments
            .Include(c => c.Replies)
                .ThenInclude(r => r.Replies)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.Files)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (parent == null)
        {
            return NotFound();
        }

        var replyTrees = parent.Replies
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReplyDto
            {
                Id = r.Id,
                Text = r.Text,
                CreatedAt = r.CreatedAt,
                UserName = r.User.UserName,
                Email = r.User.Email,
                HomePage = r.User.HomePage,
                RepliesCount = r.Replies.Count,
                Files = r.Files.Select(f => new UploadedFileDto
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    ContentType = f.ContentType
                }).ToList()
            })
            .ToList();

        return Ok(replyTrees);
    }

    private static byte[] ResizeImageIfNeeded(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.Load<Rgba32>(inputStream);

        if (image.Width <= maxWidth && image.Height <= maxHeight)
        {
            return imageBytes;
        }

        image.Mutate(context =>
        {
            context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            });
        });

        using var outputStream = new MemoryStream();
        image.SaveAsPng(outputStream);

        return outputStream.ToArray();
    }

    private CommentTreeDto BuildCommentTree(Comment comment)
    {
        return new CommentTreeDto
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UserName = comment.User.UserName,
            Email = comment.User.Email,
            HomePage = comment.User.HomePage,
            Files = comment.Files.Select(f => new FileMetaDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType
            }).ToList(),
            Replies = comment.Replies
                .OrderByDescending(r => r.CreatedAt)
                .Select(BuildCommentTree)
                .ToList()
        };
    }
}
