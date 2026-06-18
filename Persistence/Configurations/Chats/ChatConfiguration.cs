using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Aggregates.Chats;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Persistence.Configurations.Chats;

public sealed class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("chats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasConversion<short>();

        builder.Property(x => x.Title)
            .HasMaxLength(200);

        builder.Property(x => x.Username)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.IsPublic);

        builder.HasIndex(x => x.Type);

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasOne<File>()
            .WithMany()
            .HasForeignKey(x => x.PhotoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Participants)
            .WithOne()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Participants)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.PinnedMessages)
            .WithOne()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.PinnedMessages)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}