using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.JoinChannel;

public sealed record JoinChannelCommand(
    Guid ChatId
) : IAppRequest;
