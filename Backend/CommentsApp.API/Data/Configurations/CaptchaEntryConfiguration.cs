using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommentsApp.API.Models;

namespace CommentsApp.API.Data.Configurations;

public class CaptchaEntryConfiguration : IEntityTypeConfiguration<CaptchaEntry>
{
    public void Configure(EntityTypeBuilder<CaptchaEntry> builder)
    {
        builder
            .Property(c => c.Code)
            .HasMaxLength(10);
    }
}
