using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Domain.Interface.Services.v1;

public interface IDownloaderVideoService
{
    OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url);

    Task<OperationResult<string>> DownloadVideo(string url, string quality);

    Task<OperationResult<string>> GetVideoDownloadUrl(string fileName);
}
