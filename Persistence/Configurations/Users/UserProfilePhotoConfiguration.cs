using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Persistence.Configurations.Users;

public sealed class UserProfilePhotoConfiguration : IEntityTypeConfiguration<UserProfilePhoto>
{
    public void Configure(EntityTypeBuilder<UserProfilePhoto> builder)
    {
        builder.ToTable("user_profile_photos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.FileId);

        builder.HasOne<File>()
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
