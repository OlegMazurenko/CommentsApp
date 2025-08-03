using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommentsApp.API.Models;

namespace CommentsApp.API.Data.Configurations;

public class FileUploadConfiguration : IEntityTypeConfiguration<FileUpload>
{
    public void Configure(EntityTypeBuilder<FileUpload> builder)
    {
        builder
            .HasOne(f => f.Comment)
            .WithMany(c => c.Files)
            .HasForeignKey(f => f.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
