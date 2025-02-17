using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Application.AppServices.v1.Interfaces;

public interface IDownloaderVideoAppServices
{
    Task<OperationResult<string>> DownloadVideo(string url, string quality);

    OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url);

    Task<OperationResult<string>> GetVideoDownloadUrl(string fileName);
}
