using Messenger.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Messenger.Infrastructure.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId => Guid.Parse(
        User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException());

    public Guid SessionId => Guid.Parse(
        User?.FindFirstValue(ClaimTypes.Sid)
        ?? throw new UnauthorizedAccessException());

    public string PhoneNumber =>
        User?.FindFirstValue(ClaimTypes.MobilePhone)
        ?? throw new UnauthorizedAccessException();

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;
}
