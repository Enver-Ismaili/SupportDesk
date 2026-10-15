using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Reference).IsRequired().HasMaxLength(32);
        builder.HasIndex(t => t.Reference).IsUnique();
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(4000);
        builder.Property(t => t.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(t => t.CustomerEmail).IsRequired().HasMaxLength(256);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.LastModifiedAt).IsRequired();
        builder.Property(t => t.DueAt).IsRequired();

        builder.HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey(t => t.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Comments)
            .WithOne()
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(t => t.Comments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
