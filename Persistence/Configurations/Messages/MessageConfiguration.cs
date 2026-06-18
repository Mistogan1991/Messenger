using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.Configurations.Messages;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasConversion<short>();

        builder.Property(x => x.Content)
            .HasColumnType("text");

        builder.Property(x => x.IsEdited)
            .IsRequired();

        builder.HasIndex(x => x.ChatId);

        builder.HasIndex(x => x.SenderId);

        builder.HasIndex(x => new { x.ChatId, x.CreatedAtUtc })
            .HasDatabaseName("IX_messages_chat_sent_at");

        builder.HasOne<Chat>()
            .WithMany()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(x => x.ReplyToMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(x => x.ForwardedFromMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ForwardedFromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Chat>()
            .WithMany()
            .HasForeignKey(x => x.ForwardedFromChatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Attachments)
            .WithOne()
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Attachments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Reactions)
            .WithOne()
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Reactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
