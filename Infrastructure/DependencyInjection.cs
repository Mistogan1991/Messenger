using MassTransit;
using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Application.Abstractions.Storage;
using Messenger.Application.Common.Messaging;
using Messenger.Infrastructure.Authentication;
using Messenger.Infrastructure.Messaging;
using Messenger.Infrastructure.Messaging.Consumers;
using Messenger.Infrastructure.Security;
using Messenger.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Minio;

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

        AddMinioStorage(builder);
        AddMessaging(builder);
    }

    private static void AddMessaging(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

        var rabbit = builder.Configuration.GetSection("RabbitMq");
        var host = rabbit["Host"];

        builder.Services.AddMassTransit(bus =>
        {
            bus.AddConsumer<MessageSentConsumer>();

            if (!string.IsNullOrWhiteSpace(host))
            {
                // Real broker when configured.
                bus.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, h =>
                    {
                        h.Username(rabbit["Username"] ?? "guest");
                        h.Password(rabbit["Password"] ?? "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                // In-process transport so the app runs locally without RabbitMQ.
                bus.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
            }
        });
    }

    private static void AddMinioStorage(IHostApplicationBuilder builder)
    {
        builder.Services.Configure<MinioOptions>(
            builder.Configuration.GetSection(MinioOptions.SectionName));

        builder.Services.AddSingleton<IMinioClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;

            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.UseSsl)
                .Build();
        });

        builder.Services.AddScoped<IFileStorage, MinioFileStorage>();
    }
}
