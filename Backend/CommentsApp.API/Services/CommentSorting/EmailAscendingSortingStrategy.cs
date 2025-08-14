using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public class EmailAscendingSortingStrategy : ICommentSortingStrategy
{
    public string SortingType => "email_asc";

    public IQueryable<Comment> Sort(IQueryable<Comment> comments) =>
        comments.OrderBy(c => c.User.Email);
}
