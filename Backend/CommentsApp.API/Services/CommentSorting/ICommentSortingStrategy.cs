using CommentsApp.API.Models;

namespace CommentsApp.API.Services.CommentSorting;

public interface ICommentSortingStrategy
{
    string SortingType { get; }

    IQueryable<Comment> Sort(IQueryable<Comment> comments);
}
