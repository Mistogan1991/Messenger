using Messenger.API.Contracts.Messages;
using Messenger.Application.Features.Messages.Commands.AddAttachment;
using Messenger.Application.Features.Messages.Commands.AddReaction;
using Messenger.Application.Features.Messages.Commands.EditMessage;
using Messenger.Application.Features.Messages.Commands.ForwardMessage;
using Messenger.Application.Features.Messages.Commands.ReadMessage;
using Messenger.Application.Features.Messages.Commands.RemoveReaction;
using Messenger.Application.Features.Messages.Commands.ReplyMessage;
using Messenger.Application.Features.Messages.Commands.SendMessage;

namespace Messenger.API.Mappings;

public static class MessageMappings
{
    public static SendMessageCommand ToCommand(this SendMessageRequest request)
    {
        return new SendMessageCommand(
            request.ChatId,
            request.Content);
    }

    public static EditMessageCommand ToCommand(this EditMessageRequest request, Guid messageId)
    {
        return new EditMessageCommand(
            messageId,
            request.Content);
    }

    public static ReplyMessageCommand ToCommand(this ReplyMessageRequest request)
    {
        return new ReplyMessageCommand(
            request.ChatId,
            request.ReplyToMessageId,
            request.Content);
    }

    public static ForwardMessageCommand ToCommand(this ForwardMessageRequest request)
    {
        return new ForwardMessageCommand(
            request.TargetChatId,
            request.SourceMessageId);
    }

    public static ReadMessageCommand ToCommand(this ReadMessageRequest request)
    {
        return new ReadMessageCommand(
            request.ChatId,
            request.MessageId);
    }

    public static AddReactionCommand ToCommand(this AddReactionRequest request)
    {
        return new AddReactionCommand (
            request.MessageId,
            request.Emoji);
    }

    public static RemoveReactionCommand ToCommand(this RemoveReactionRequest request)
    {
        return new RemoveReactionCommand(
            request.MessageId,
            request.Emoji);
    }

    public static AddAttachmentCommand ToCommand(this AddAttachmentRequest request)
    {
        return new AddAttachmentCommand(
            request.MessageId,
            request.FileId,
            request.Caption);
    }
}
