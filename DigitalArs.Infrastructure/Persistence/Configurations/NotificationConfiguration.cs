using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalArs.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.ID_Notification);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(n => n.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(n => n.IsRead)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.ID_User)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => new { n.ID_User, n.IsRead, n.CreatedAt });
    }
}
