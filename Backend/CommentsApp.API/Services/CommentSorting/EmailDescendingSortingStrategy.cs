using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public class EmailDescendingSortingStrategy : ICommentSortingStrategy
{
    public string SortingType => "email_desc";

    public IQueryable<Comment> Sort(IQueryable<Comment> comments) =>
        comments.OrderByDescending(c => c.User.Email);
}
