using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Profile.Queries.GetMyProfile;

public sealed record GetMyProfileQuery() : IAppRequest<MyProfileDto>;

public sealed record MyProfileDto(
    Guid Id,
    string PhoneNumber,
    string? Username,
    string? FirstName,
    string? LastName,
    string? Bio
);
