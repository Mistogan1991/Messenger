using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.UnitTests.Persistence;

public class FileAccessTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"fileaccess-{Guid.NewGuid()}")
            .Options);

    private static File NewFile(Guid uploader) =>
        File.Create(uploader, "f.bin", $"key/{Guid.NewGuid():N}", "application/octet-stream", 10, FileType.Document);

    [Fact]
    public async Task Uploader_can_access_their_file()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var file = NewFile(me);
        db.Files.Add(file);
        await db.SaveChangesAsync();

        Assert.True(await new FileRepository(db).CanUserAccessAsync(file.Id, me, CancellationToken.None));
    }

    [Fact]
    public async Task Member_of_a_chat_the_file_is_attached_in_can_access_it()
    {
        var uploader = Guid.NewGuid();
        var me = Guid.NewGuid();
        await using var db = NewContext();

        var file = NewFile(uploader);
        var chat = Chat.CreateGroup("Team", me, Array.Empty<Guid>());
        var message = Message.Send(chat.Id, me, MessageType.Text, "see attached");
        db.Files.Add(file);
        db.Chats.Add(chat);
        db.Messages.Add(message);
        db.Set<MessageAttachment>().Add(MessageAttachment.Create(message.Id, file.Id, null));
        await db.SaveChangesAsync();

        Assert.True(await new FileRepository(db).CanUserAccessAsync(file.Id, me, CancellationToken.None));
    }

    [Fact]
    public async Task Profile_photos_are_accessible()
    {
        var owner = Guid.NewGuid();
        var someoneElse = Guid.NewGuid();
        await using var db = NewContext();

        var file = NewFile(owner);
        db.Files.Add(file);
        db.Set<UserProfilePhoto>().Add(UserProfilePhoto.Create(owner, file.Id));
        await db.SaveChangesAsync();

        Assert.True(await new FileRepository(db).CanUserAccessAsync(file.Id, someoneElse, CancellationToken.None));
    }

    [Fact]
    public async Task Unrelated_user_cannot_access_a_file()
    {
        var uploader = Guid.NewGuid();
        var stranger = Guid.NewGuid();
        await using var db = NewContext();
        var file = NewFile(uploader);
        db.Files.Add(file);
        await db.SaveChangesAsync();

        Assert.False(await new FileRepository(db).CanUserAccessAsync(file.Id, stranger, CancellationToken.None));
    }
}
