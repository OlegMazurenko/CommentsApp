using CommentsApp.API.Models;

using Microsoft.EntityFrameworkCore;

namespace CommentsApp.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<FileUpload> FileUploads => Set<FileUpload>();
    public DbSet<CaptchaEntry> Captchas => Set<CaptchaEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
