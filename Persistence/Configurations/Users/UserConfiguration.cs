using Messenger.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.Configurations.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        builder.Property(x => x.Bio)
            .HasMaxLength(500);

        builder.Property(x => x.PhoneNumber)
            .HasConversion(
                v => v.Value,
                v => PhoneNumber.Create(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v)
                    ? null
                    : Username.Create(v))
            .HasMaxLength(50);

        builder.OwnsOne(
            x => x.PrivacySettings,
            privacy =>
            {
                privacy.Property(x => x.LastSeen)
                    .HasConversion<short>()
                    .HasColumnName("privacy_last_seen");

                privacy.Property(x => x.PhoneNumber)
                    .HasConversion<short>()
                    .HasColumnName("privacy_phone_number");

                privacy.Property(x => x.ProfilePhoto)
                    .HasConversion<short>()
                    .HasColumnName("privacy_profile_photo");

                privacy.Property(x => x.Calls)
                    .HasConversion<short>()
                    .HasColumnName("privacy_calls");

                privacy.Property(x => x.ForwardedMessages)
                    .HasConversion<short>()
                    .HasColumnName("privacy_forwarded_messages");
            });

        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasMany(x => x.Sessions)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Sessions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Contacts)
            .WithOne()
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Contacts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Photos)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Photos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.BlockedUsers)
            .WithOne()
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.BlockedUsers)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
