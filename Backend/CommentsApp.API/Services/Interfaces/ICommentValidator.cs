using CommentsApp.API.Models;

namespace CommentsApp.API.Services.Interfaces;

public interface ICommentValidator
{
    List<string> Validate(CommentDto dto);
}