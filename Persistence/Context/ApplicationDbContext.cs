using System.Linq.Expressions;
using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Aggregates.Notifications;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.Common;
using Messenger.Persistence.Outbox;
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

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Exclude soft-deleted rows from every query by default. Use IgnoreQueryFilters() to read them.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletableEntity<Guid>).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>Builds <c>e =&gt; e.DeletedAtUtc == null</c> for the given soft-deletable entity type.</summary>
    private static LambdaExpression BuildSoftDeleteFilter(Type clrType)
    {
        var parameter = Expression.Parameter(clrType, "e");
        var property = Expression.Property(parameter, nameof(SoftDeletableEntity<Guid>.DeletedAtUtc));
        var body = Expression.Equal(property, Expression.Constant(null, typeof(DateTime?)));

        return Expression.Lambda(body, parameter);
    }

    /// <summary>
    /// Persists raised domain events into the outbox within the same transaction as the aggregate
    /// changes. Publication happens asynchronously and at-least-once via <see cref="OutboxProcessor"/>.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var outboxMessages = aggregatesWithEvents
            .SelectMany(a => a.DomainEvents)
            .Select(OutboxMessage.Create)
            .ToList();

        if (outboxMessages.Count != 0)
            OutboxMessages.AddRange(outboxMessages);

        foreach (var aggregate in aggregatesWithEvents)
            aggregate.ClearDomainEvents();

        return base.SaveChangesAsync(cancellationToken);
    }
}
