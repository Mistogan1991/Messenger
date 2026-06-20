using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Messenger.API.Extensions;

public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry tracing + metrics for ASP.NET Core, HttpClient and the runtime. Telemetry
    /// is exported via OTLP only when <c>OpenTelemetry:OtlpEndpoint</c> is configured; otherwise it
    /// is collected but not exported (so local runs need no collector).
    /// </summary>
    public static void AddOpenTelemetryObservability(this IHostApplicationBuilder builder)
    {
        var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"];

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: "Messenger.API",
                serviceVersion: typeof(OpenTelemetryExtensions).Assembly.GetName().Version?.ToString()))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            });
    }
}
