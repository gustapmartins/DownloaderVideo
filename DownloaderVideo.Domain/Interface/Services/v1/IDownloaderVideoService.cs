using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Domain.Interface.Services.v1;

public interface IDownloaderVideoService
{
    Task<OperationResult<List<DownloaderVideoEntity>>> GetAvailableQualitiesAsync(string url);

    OperationResult<Stream> DownloadVideo(string url, string quality);
}
