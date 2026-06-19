using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.API.Extensions;

public static class MigrationExtensions
{
    /// <summary>
    /// Applies pending EF Core migrations on startup, retrying briefly so the API can come up
    /// alongside its database (e.g. under docker-compose) without a race.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app, int retries = 10)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = app.Logger;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied.");
                return;
            }
            catch (Exception ex) when (attempt < retries)
            {
                logger.LogWarning(ex, "Migration attempt {Attempt}/{Retries} failed; retrying.", attempt, retries);
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}
