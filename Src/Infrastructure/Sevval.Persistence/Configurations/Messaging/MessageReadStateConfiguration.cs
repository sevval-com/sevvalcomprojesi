using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevval.Domain.Messaging;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Persistence.Configurations.Messaging;

public class MessageReadStateConfiguration : IEntityTypeConfiguration<MessageReadState>
{
    public void Configure(EntityTypeBuilder<MessageReadState> builder)
    {
        builder.ToTable("MessageReadStates");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ReaderId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.MessageId)
            .IsRequired();

        builder.HasIndex(s => new { s.MessageId, s.ReaderId })
            .IsUnique();

        builder.HasOne<MessagingMessage>()
            .WithMany()
            .HasForeignKey(s => s.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
