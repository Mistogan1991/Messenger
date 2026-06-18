using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Profile.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
    string FirstName,
    string? LastName,
    string? Bio
) : IAppRequest;
