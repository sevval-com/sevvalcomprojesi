using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Persistence.Configurations.Messaging;

public class MessagingMessageConfiguration : IEntityTypeConfiguration<MessagingMessage>
{
    public void Configure(EntityTypeBuilder<MessagingMessage> builder)
    {
        builder.ToTable("MessagingMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(m => m.RecipientId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(m => m.Body)
            .IsRequired();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.MessageType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.ListingId)
            .IsRequired(false);

        builder.HasIndex(m => new { m.SenderId, m.RecipientId, m.CreatedOnUtc });
        builder.HasIndex(m => new { m.RecipientId, m.SenderId, m.CreatedOnUtc });
        builder.HasIndex(m => new { m.ListingId, m.SenderId, m.RecipientId, m.CreatedOnUtc });
        builder.HasIndex(m => new { m.ListingId, m.RecipientId, m.SenderId, m.CreatedOnUtc });
    }
}
