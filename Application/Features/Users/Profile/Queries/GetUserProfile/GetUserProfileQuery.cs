using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Profile.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(
    Guid UserId
) : IAppRequest<UserProfileDto>;

public sealed record UserProfileDto(
    Guid Id,
    string? Username,
    string? FirstName,
    string? LastName,
    string? Bio
);
