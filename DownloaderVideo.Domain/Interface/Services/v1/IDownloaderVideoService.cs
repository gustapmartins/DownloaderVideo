using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Domain.Interface.Services.v1;

public interface IDownloaderVideoService
{
    OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url);

    OperationResult<Stream> DownloadVideo(string url, string quality);
}
