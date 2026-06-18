namespace Messenger.Application.Common.Exceptions;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";

    public const string Unauthorized = "UNAUTHORIZED";

    public const string Forbidden = "FORBIDDEN";

    public const string NotFound = "NOT_FOUND";

    public const string UserNotFound = "USER_NOT_FOUND";

    public const string ServerError = "SERVER_ERROR";

    public const string InvalidOtp = "INVALID_OTP";

    public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";

    public const string SessionExpired = "SESSION_EXPIRED";
}
