using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Chat> Chats => Set<Chat>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<Domain.Aggregates.Files.File> Files => Set<Domain.Aggregates.Files.File>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //{
        //    if (typeof(SoftDeletableEntity<Guid>).IsAssignableFrom(entityType.ClrType))
        //    {
        //        modelBuilder.Entity(entityType.ClrType)
        //            .HasQueryFilter(GenerateFilterExpression(entityType.ClrType));
        //    }
        //}

        base.OnModelCreating(modelBuilder);
    }
}
