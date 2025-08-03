using CommentsApp.API.Models;

namespace CommentsApp.API.Services.Interfaces;

public interface IUserService
{
    Task<User> PrepareUserAsync(string email, string userName, string? homePage);
}