using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Users.Dtos;

namespace Messenger.Application.Features.Users.Privacy.Queries.GetPrivacySettings;

public sealed record GetPrivacySettingsQuery() : IAppRequest<PrivacySettingsDto>;


