using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Aggregates.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Persistence.Configurations.Messages;

public sealed class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("message_attachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Caption)
            .HasColumnType("text");

        builder.HasIndex(x => x.MessageId);

        builder.HasIndex(x => x.FileId);

        builder.HasOne<File>()
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
