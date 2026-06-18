using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Auth.Commands.RequestOtp;

public sealed record RequestOtpCommand(string PhoneNumber) : IAppRequest<RequestOtpCommandResult>;

public sealed record RequestOtpCommandResult(string DevelopmentCode);

