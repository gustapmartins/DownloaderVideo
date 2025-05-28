using DownloaderVideo.Application.AppServices.v1.Interfaces;
using DownloaderVideo.Application.AppServices.v1;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Application.AppServices;

[ExcludeFromCodeCoverage]
public static class AppServiceDependencyInjection
{
    public static void AppServiceDependencyInjectionModule(this IServiceCollection services)
    {
        services.AddScoped<IDownloaderVideoAppServices, DownloaderVideoAppServices>();
    }
}
