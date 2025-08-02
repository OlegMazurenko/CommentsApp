using CommentsApp.API.Models;

namespace CommentsApp.API.Services.Interfaces;

public interface ICommentService
{
    Task<int> CreateCommentAsync(CommentDto dto);
    Task<(IList<CommentListItemDto> Comments, bool IsLastPage)> GetCommentsAsync(int page, string sort);
    Task<IEnumerable<ReplyDto>> GetRepliesAsync(int parentId);
}