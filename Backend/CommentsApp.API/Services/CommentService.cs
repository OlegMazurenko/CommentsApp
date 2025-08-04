using CommentsApp.API.Data;
using CommentsApp.API.Hubs;
using CommentsApp.API.Models;
using CommentsApp.API.Services.Interfaces;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.API.Services;

public class CommentService(AppDbContext context,
    IHtmlSanitizer sanitizer,
    IHubContext<CommentsHub> hubContext,
    ICacheService cacheService,
    IFileService fileService,
    ICaptchaService captchaService,
    ICommentValidator commentValidator,
    IUserService userService,
    FileProcessorQueue fileProcessorQueue) : ICommentService
{
    private readonly AppDbContext _context = context;
    private readonly IHtmlSanitizer _sanitizer = sanitizer;
    private readonly IHubContext<CommentsHub> _hubContext = hubContext;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IFileService _fileService = fileService;
    private readonly ICaptchaService _captchaService = captchaService;
    private readonly ICommentValidator _commentValidator = commentValidator;
    private readonly IUserService _userService = userService;
    private readonly FileProcessorQueue _fileProcessorQueue = fileProcessorQueue;

    public async Task<int> CreateCommentAsync(CommentDto dto)
    {
        var errors = _commentValidator.Validate(dto);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join("; ", errors));
        }

        if (!await _captchaService.ValidateCaptchaAsync(dto.CaptchaId, dto.CaptchaCode))
        {
            throw new InvalidOperationException("Invalid or expired CAPTCHA");
        }

        var parentComment = dto.ParentCommentId.HasValue
            ? await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == dto.ParentCommentId.Value)
            : null;

        if (dto.ParentCommentId.HasValue && parentComment == null)
        {
            throw new InvalidOperationException("Parent comment not found");
        }

        var user = await _userService.PrepareUserAsync(dto.Email, dto.UserName, dto.HomePage);

        var comment = new Comment
        {
            Text = _sanitizer.Sanitize(dto.Text),
            User = user,
            ParentCommentId = dto.ParentCommentId
        };

        var files = _fileService.ValidateAndProcessFiles(dto.Files, comment);
        _context.FileUploads.AddRange(files);
        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();
        _fileProcessorQueue.Enqueue(comment);
        _cacheService.ClearCommentCache();

        await _hubContext.Clients.All.SendAsync("NewComment", comment.Id);

        return comment.Id;
    }

    public async Task<(IList<CommentListItemDto> Comments, bool IsLastPage)> GetCommentsAsync(int page, string sort)
    {
        const int pageSize = 25;
        var cacheKey = $"comments:page:{page}:sort:{sort}";
        var cached = _cacheService.GetFromCache(cacheKey);

        if (cached is not null)
        {
            return ((List<CommentListItemDto>, bool))cached;
        }

        var query = _context.Comments
            .AsNoTracking()
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
            .Select(c => new CommentListItemDto
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

        _cacheService.SetToCache(cacheKey, (comments, isLastPage), TimeSpan.FromSeconds(60));

        return (comments, isLastPage);
    }

    public async Task<IEnumerable<ReplyDto>> GetRepliesAsync(int parentId)
    {
        var parent = await _context.Comments
            .AsNoTracking()
            .Include(c => c.Replies)
                .ThenInclude(r => r.Replies)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.Files)
            .FirstOrDefaultAsync(c => c.Id == parentId);

        return parent == null
            ? throw new InvalidOperationException("Parent comment not found")
            : parent.Replies
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
    }
}
