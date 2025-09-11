namespace CommentsApp.API.Services.CommentSorting;

public class CommentSortingResolver(IEnumerable<ICommentSortingStrategy> strategies) : ICommentSortingResolver
{
    private readonly IEnumerable<ICommentSortingStrategy> _strategies = strategies;

    public ICommentSortingStrategy Resolve(string type) =>
        _strategies.FirstOrDefault(s => s.SortingType == type)
        ?? _strategies.First(s => s.SortingType == "date_desc");
}
