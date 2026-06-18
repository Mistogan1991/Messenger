using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Chats.Commands.StartPrivateChat;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.ValueObjects;
using Xunit;

namespace Messenger.UnitTests.Application;

public class StartPrivateChatHandlerTests
{
    private static User SomeUser() => User.Create(PhoneNumber.Create("+12345678901"));

    private sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid UserId { get; init; }
        public Guid SessionId => Guid.Empty;
        public string PhoneNumber => "+12345678901";
        public bool IsAuthenticated => true;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeChatRepository : IChatRepository
    {
        public Chat? ExistingPrivateChat { get; set; }
        public Chat? Added { get; private set; }

        public Task AddAsync(Chat chat, CancellationToken ct) { Added = chat; return Task.CompletedTask; }
        public Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult<Chat?>(null);
        public Task<Chat?> GetPrivateChatAsync(Guid userA, Guid userB, CancellationToken ct) =>
            Task.FromResult(ExistingPrivateChat);
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

    private static StartPrivateChatHandler NewHandler(
        FakeCurrentUser currentUser, FakeChatRepository chats, FakeUserRepository users, FakeUnitOfWork uow) =>
        new(uow, currentUser, chats, users);

    [Fact]
    public async Task Fails_when_target_is_self()
    {
        var me = Guid.NewGuid();
        var uow = new FakeUnitOfWork();
        var handler = NewHandler(new FakeCurrentUser { UserId = me }, new FakeChatRepository(),
            new FakeUserRepository(), uow);

        var result = await handler.Handle(new StartPrivateChatCommand(me), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task Fails_when_other_user_does_not_exist()
    {
        var uow = new FakeUnitOfWork();
        var handler = NewHandler(new FakeCurrentUser { UserId = Guid.NewGuid() }, new FakeChatRepository(),
            new FakeUserRepository { UserById = null }, uow);

        var result = await handler.Handle(new StartPrivateChatCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task Returns_existing_chat_without_creating_a_new_one()
    {
        var me = Guid.NewGuid();
        var other = Guid.NewGuid();
        var existing = Chat.CreatePrivate(me, other);
        var chats = new FakeChatRepository { ExistingPrivateChat = existing };
        var uow = new FakeUnitOfWork();

        var handler = NewHandler(new FakeCurrentUser { UserId = me }, chats,
            new FakeUserRepository { UserById = SomeUser() }, uow);

        var result = await handler.Handle(new StartPrivateChatCommand(other), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(existing.Id, result.Data);
        Assert.Null(chats.Added);
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task Creates_and_persists_a_new_private_chat()
    {
        var me = Guid.NewGuid();
        var other = Guid.NewGuid();
        var chats = new FakeChatRepository { ExistingPrivateChat = null };
        var uow = new FakeUnitOfWork();

        var handler = NewHandler(new FakeCurrentUser { UserId = me }, chats,
            new FakeUserRepository { UserById = SomeUser() }, uow);

        var result = await handler.Handle(new StartPrivateChatCommand(other), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(chats.Added);
        Assert.Equal(chats.Added!.Id, result.Data);
        Assert.Equal(1, uow.SaveCount);
    }
}
