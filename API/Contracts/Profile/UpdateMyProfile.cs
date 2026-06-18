namespace Messenger.API.Contracts.Profile;

public sealed record UpdateMyProfileRequest(
    string FirstName,
    string? LastName,
    string? Bio
);
