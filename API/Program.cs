using Messenger.API;
using Messenger.API.Extensions;
using Messenger.API.Hubs;
using Messenger.API.Middlewares;
using Messenger.API.Swagger;
using Messenger.Application;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Logging;
using Messenger.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSerilogLogging();
builder.AddOpenTelemetryObservability();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddPersistenceServices();
builder.AddApiServices();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await app.ApplyMigrationsAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseSwaggerConfiguration();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();
