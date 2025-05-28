using DownloaderVideo.Application.Controllers.v1;
using DownloaderVideo.Domain.Interface.Services;
using DownloaderVideo.Domain.Interface.Services.v1;
using DownloaderVideo.Domain.Interface.Utils;
using DownloaderVideo.Domain.Services.v1;
using DownloaderVideo.Domain.Utils;
using DownloaderVideo.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using VarzeaLeague.Domain.Service;
using YoutubeExplode;

namespace DownloaderVideo.Domain;

[ExcludeFromCodeCoverage]
public static class ServiceDependencyInjection
{
    public static void ServiceDependencyInjectionModule(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IDownloaderVideoService, DownloaderVideoService>();

        services.AddScoped<IMemoryCacheService, MemoryCacheService>();

        services.AddScoped<IGenerateHash, GenerateHash>();

        services.AddScoped<IRedisService, RedisService>();

        services.AddScoped<INotificationBase, NotificationBase>();

        services.AddScoped<YoutubeClient>();
    }
}
