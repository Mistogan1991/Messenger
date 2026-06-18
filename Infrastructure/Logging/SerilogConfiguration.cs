using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Messenger.Infrastructure.Logging;

public static class SerilogConfiguration
{
    /// <summary>
    /// Registers Serilog as the application logging provider with structured console output.
    /// Configuration is code-based (no external sink packages) to keep the dependency surface small;
    /// switching to <c>ReadFrom.Configuration</c> is a later enhancement.
    /// </summary>
    public static void AddSerilogLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, configuration) => configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .ReadFrom.Services(services)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));
    }
}
