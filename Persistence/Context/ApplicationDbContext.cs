using Messenger.Application.Common.Messaging;
using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Context;

public sealed class ApplicationDbContext : DbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventDispatcher? dispatcher = null) : base(options)
    {
        _dispatcher = dispatcher;
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregatesWithEvents
            .SelectMany(a => a.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var aggregate in aggregatesWithEvents)
            aggregate.ClearDomainEvents();

        if (_dispatcher is not null && domainEvents.Count != 0)
            await _dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return result;
    }
}
