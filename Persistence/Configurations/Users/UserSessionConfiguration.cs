using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.Configurations.Users;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DeviceId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DeviceName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DeviceType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RefreshTokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.LastActivityAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.DeviceId);
    }
}