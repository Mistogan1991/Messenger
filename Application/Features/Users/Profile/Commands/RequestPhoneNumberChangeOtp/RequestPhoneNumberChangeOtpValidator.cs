using FluentValidation;
using Messenger.Application.Features.Auth.Commands.RequestOtp;

namespace Messenger.Application.Features.Users.Profile.Commands.RequestPhoneNumberChangeOtp;

public sealed class RequestPhoneNumberChangeOtpValidator : AbstractValidator<RequestOtpCommand>
{
    public RequestPhoneNumberChangeOtpValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9]{10,15}$")
            .WithMessage("Invalid phone number.");
    }
}