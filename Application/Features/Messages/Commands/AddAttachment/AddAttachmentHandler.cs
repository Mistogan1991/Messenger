using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Messages.Commands.AddAttachment
{
    public sealed class AddAttachmentHandler
    : IAppRequestHandler<AddAttachmentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IFileRepository _fileRepository;
        private readonly IMessageRepository _messageRepository;

        public AddAttachmentHandler(
            IUnitOfWork unitOfWork,
            //IFileRepository fileRepository,
            IMessageRepository messageRepository)
        {
            _unitOfWork = unitOfWork;
            //_fileRepository = fileRepository;
            _messageRepository = messageRepository;
        }

        public async Task<Result> Handle(
            AddAttachmentCommand request,
            CancellationToken ct)
        {
            var message =
                await _messageRepository.GetByIdAsync(
                    request.MessageId,
                    ct);

            if (message is null)
                return Result.Failure(["Message not found"]);

            //var file = await _fileRepository.GetByIdAsync(request.FileId, ct);

            //if (file is null)
            //    return Result.Failure(["File not found"]);

            message.AddAttachment(
                request.FileId,
                request.Caption);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
