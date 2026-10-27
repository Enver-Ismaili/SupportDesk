using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);
        // The domain assigns the id before the comment is added to a tracked ticket.
        // A store-generated Guid would be treated as an existing row and updated instead of inserted.
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.AuthorName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Body).IsRequired().HasMaxLength(4000);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.HasIndex(c => c.TicketId);
    }
}
