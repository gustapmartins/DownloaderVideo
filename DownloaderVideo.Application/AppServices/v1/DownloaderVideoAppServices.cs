using AutoMapper;
using DownloaderVideo.Application.AppServices.v1.Interfaces;
using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Application.AppServices.v1;

[ExcludeFromCodeCoverage]
public class DownloaderVideoAppServices(
    ILogger<DownloaderVideoAppServices> _logger,
    IDownloaderVideoService _generateTemplateService,
    IMapper _mapper) : IDownloaderVideoAppServices
{
    public OperationResult<Stream> DownloadVideo(string url, string quality)
    {
        OperationResult<Stream> result = _generateTemplateService.DownloadVideo(url, quality);
        return result;
    }

    public async Task<OperationResult<List<DownloaderVideoEntity>>> GetAvailableQualitiesAsync(string url)
    {
        OperationResult<List<DownloaderVideoEntity>> result = await _generateTemplateService.GetAvailableQualitiesAsync(url);
        return result;
    }
}
