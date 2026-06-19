using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Users.Presence.Commands.MarkUserOffline;
using Messenger.Application.Features.Users.Presence.Queries.GetUserPresence;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.ValueObjects;
using Xunit;

namespace Messenger.UnitTests.Application;

public class PresenceTests
{
    private static User NewUser() => User.Create(PhoneNumber.Create("+12345678901"));

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public Task<int> SaveChangesAsync(CancellationToken ct = default) { SaveCount++; return Task.FromResult(1); }
    }

    private sealed class FakePresenceTracker : IPresenceTracker
    {
        public bool Online { get; set; }
        public Task<bool> UserConnectedAsync(Guid userId) => Task.FromResult(true);
        public Task<bool> UserDisconnectedAsync(Guid userId) => Task.FromResult(true);
        public Task<bool> IsOnlineAsync(Guid userId) => Task.FromResult(Online);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? UserById { get; set; }
        public Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default) => Task.FromResult(UserById);

        public Task AddAsync(User user, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetByIdWithContactsAsync(Guid userId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetByIdWithBlockedUsersAsync(Guid userId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetByIdWithSessionsAsync(Guid userId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetByIdWithPrivacySettingsAsync(Guid userId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetBySessionAsync(Guid sessionId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<bool> ExistsUsernameAsync(Username username, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<List<Guid>> GetExistingUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default) => throw new NotSupportedException();
    }

    [Fact]
    public void SetLastSeen_stamps_the_last_seen_time()
    {
        var user = NewUser();
        Assert.Null(user.LastSeenAt);

        user.SetLastSeen();

        Assert.NotNull(user.LastSeenAt);
    }

    [Fact]
    public async Task MarkUserOffline_stamps_last_seen_and_saves()
    {
        var user = NewUser();
        var uow = new FakeUnitOfWork();
        var handler = new MarkUserOfflineHandler(new FakeUserRepository { UserById = user }, uow);

        var result = await handler.Handle(new MarkUserOfflineCommand(user.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(user.LastSeenAt);
        Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task MarkUserOffline_is_a_noop_when_the_user_is_missing()
    {
        var uow = new FakeUnitOfWork();
        var handler = new MarkUserOfflineHandler(new FakeUserRepository { UserById = null }, uow);

        var result = await handler.Handle(new MarkUserOfflineCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task GetUserPresence_reports_online_state_and_last_seen()
    {
        var user = NewUser();
        user.SetLastSeen();
        var handler = new GetUserPresenceHandler(
            new FakeUserRepository { UserById = user }, new FakePresenceTracker { Online = true });

        var result = await handler.Handle(new GetUserPresenceQuery(user.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data.IsOnline);
        Assert.Equal(user.LastSeenAt, result.Data.LastSeenAtUtc);
    }

    [Fact]
    public async Task GetUserPresence_fails_when_the_user_is_missing()
    {
        var handler = new GetUserPresenceHandler(
            new FakeUserRepository { UserById = null }, new FakePresenceTracker());

        var result = await handler.Handle(new GetUserPresenceQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
