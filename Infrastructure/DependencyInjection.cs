using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Infrastructure.Authentication;
using Messenger.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messenger.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IHashProvider, Sha256HashProvider>();
        builder.Services.AddScoped<IOtpProvider, OtpProvider>();
        builder.Services.AddScoped<IJwtProvider, JwtProvider>();
        builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
    }
}
