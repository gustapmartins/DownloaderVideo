using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using DownloaderVideo.Domain.Interface.Dao;
using DownloaderVideo.Infra.Data.Repository.EfCore;

namespace DownloaderVideo.Infra.Data;

[ExcludeFromCodeCoverage]
public static class RepositoryDependencyInjectionModule
{
    public static void RepositoryDependencyInjectionModuleModule(this IServiceCollection services)
    {
        services.AddSingleton<IDownloaderVideoDao, DownloaderVideoEfCore>();
    }
}
