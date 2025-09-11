using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public class UserAscendingSortingStrategy : ICommentSortingStrategy
{
    public string SortingType => "user_asc";

    public IQueryable<Comment> Sort(IQueryable<Comment> comments) =>
        comments.OrderBy(c => c.User.UserName);
}
