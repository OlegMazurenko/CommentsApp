using CommentsApp.API.Models;

using Microsoft.EntityFrameworkCore;

namespace CommentsApp.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<FileUpload> FileUploads => Set<FileUpload>();
    public DbSet<CaptchaEntry> Captchas => Set<CaptchaEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: maybe move to separate files
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileUpload>()
            .HasOne(f => f.Comment)
            .WithMany(c => c.Files)
            .HasForeignKey(f => f.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CaptchaEntry>()
            .Property(c => c.Code)
            .HasMaxLength(10);
    }
}
