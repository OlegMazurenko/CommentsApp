using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public class DateAscendingSortingStrategy : ICommentSortingStrategy
{
    public string SortingType => "date_asc";

    public IQueryable<Comment> Sort(IQueryable<Comment> comments) =>
        comments.OrderBy(c => c.CreatedAt);
}
