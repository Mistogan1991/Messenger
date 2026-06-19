using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Messenger.API.Extensions;

public static class RateLimitingExtensions
{
    /// <summary>Tight per-client policy for OTP requests — curbs SMS/abuse spam.</summary>
    public const string OtpPolicy = "otp";

    public static void AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Generous global safety net, partitioned per client (user id when authenticated, else IP).
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    ClientKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // Stricter window for OTP requests, applied via [EnableRateLimiting(OtpPolicy)].
            options.AddPolicy(OtpPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    ClientKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0
                    }));
        });
    }

    private static string ClientKey(HttpContext context) =>
        context.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? context.Connection.RemoteIpAddress?.ToString()
        ?? "unknown";
}
