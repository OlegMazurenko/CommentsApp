namespace CommentsApp.API.Services.CommentSorting;

public interface ICommentSortingResolver
{
    ICommentSortingStrategy Resolve(string name);
}
