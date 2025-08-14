using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public class UserDescendingSortingStrategy : ICommentSortingStrategy
{
    public string SortingType => "user_desc";

    public IQueryable<Comment> Sort(IQueryable<Comment> comments) =>
        comments.OrderByDescending(c => c.User.UserName);
}
