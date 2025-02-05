using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Application.AppServices.v1.Interfaces;

public interface IDownloaderVideoAppServices
{
    Task<OperationResult<byte[]>> DownloadVideo(string url, string quality);

    OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url);
}
