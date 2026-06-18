using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Persistence.Configurations.Files;

public sealed class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("files");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FileName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UploadedAtUtc)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<short>();

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.ThumbnailStorageKey)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.UploadedBy);

        builder.HasIndex(x => x.Type);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
