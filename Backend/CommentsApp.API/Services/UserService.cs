using CommentsApp.API.Data;
using CommentsApp.API.Models;
using CommentsApp.API.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace CommentsApp.API.Services;

public class UserService(AppDbContext context) : IUserService
{
    private readonly AppDbContext _context = context;

    public async Task<User> GetOrCreateUserAsync(string email, string userName, string? homePage)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user != null)
        {
            return user;
        }

        user = new User
        {
            Email = email,
            UserName = userName,
            HomePage = homePage
        };

        _context.Users.Add(user);

        return user;
    }
}
