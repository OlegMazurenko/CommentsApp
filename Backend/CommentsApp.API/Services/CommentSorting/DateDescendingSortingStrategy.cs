using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public class DateDescendingSortingStrategy : ICommentSortingStrategy
{
    public string SortingType => "date_desc";

    public IQueryable<Comment> Sort(IQueryable<Comment> comments) =>
        comments.OrderByDescending(c => c.CreatedAt);
}
