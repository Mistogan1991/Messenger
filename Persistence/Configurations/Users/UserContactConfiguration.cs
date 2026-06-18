using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.Configurations.Users;

public sealed class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
{
    public void Configure(EntityTypeBuilder<UserContact> builder)
    {
        builder.ToTable("user_contacts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FirstName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(200);

        builder.HasIndex(x => new { x.OwnerUserId, x.ContactUserId })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ContactUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
