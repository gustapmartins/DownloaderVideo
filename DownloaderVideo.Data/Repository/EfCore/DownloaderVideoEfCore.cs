using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Dao;
using DownloaderVideo.Infra.Data.Context;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DownloaderVideo.Infra.Data.Repository.EfCore;

public class DownloaderVideoEfCore : BaseContext<DownloaderVideoEntity>, IDownloaderVideoDao
{
    private readonly IMongoCollection<DownloaderVideoEntity> _AuthCollection;

    public DownloaderVideoEfCore(IOptions<DatabaseSettings> options) : base(options, "DownloaderVideoCollection")
    {
        _AuthCollection = Collection;
    }

    public Task<DownloaderVideoEntity> FindEmailAsync(string Email)
    {
        throw new NotImplementedException();
    }
}
