using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Domain.Interface.Services.v1;

public interface IDownloaderVideoService
{
    OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url);

    Task<OperationResult<byte[]>> DownloadVideo(string url, string quality);
}
