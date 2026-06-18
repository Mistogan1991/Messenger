using Messenger.API.Contracts.Chats;
using Messenger.Application.Features.Chats.Commands.AddMembers;
using Messenger.Application.Features.Chats.Commands.CreateChannel;
using Messenger.Application.Features.Chats.Commands.CreateGroup;
using Messenger.Application.Features.Chats.Commands.EditChat;

namespace Messenger.API.Mappings;

public static class ChatMappings
{
    public static CreateGroupCommand ToCommand(this CreateGroupRequest request)
    {
        return new CreateGroupCommand(
            request.Title,
            request.MemberIds);
    }

    public static CreateChannelCommand ToCommand(this CreateChannelRequest request)
    {
        return new CreateChannelCommand(
            request.Title,
            request.Description,
            request.IsPublic);
    }

    public static AddMembersCommand ToCommand(this AddMembersRequest request, Guid chatId)
    {
        return new AddMembersCommand(
            chatId,
            request.UserIds);
    }

    public static EditChatCommand ToCommand(this EditChatRequest request, Guid chatId)
    {
        return new EditChatCommand(
            chatId,
            request.Title,
            request.Description,
            request.IsPublic
        );
    }
}
