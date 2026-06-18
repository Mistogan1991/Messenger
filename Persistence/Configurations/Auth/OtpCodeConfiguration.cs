using Messenger.Domain.Aggregates.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.Configurations.OtpCodes;

public sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("otp_codes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CodeHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .HasConversion<short>();

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.HasIndex(x => x.PhoneNumber);

        builder.HasIndex(x => new { x.PhoneNumber, x.Purpose });
    }
}
