using AutoMapper;
using DownloaderVideo.Application.AppServices.v1.Interfaces;
using DownloaderVideo.Application.Dto.v1;
using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Application.AppServices.v1;

[ExcludeFromCodeCoverage]
public class DownloaderVideoAppServices : IDownloaderVideoAppServices
{
    private readonly ILogger<DownloaderVideoAppServices> _logger;
    private readonly IDownloaderVideoService _generateTemplateService;
    private readonly IMapper _mapper;

    public DownloaderVideoAppServices(IMapper mapper, ILogger<DownloaderVideoAppServices> logger, IDownloaderVideoService generateTemplateService)
    {
        _mapper = mapper;
        _logger = logger;
        _generateTemplateService = generateTemplateService;
    }

    public async Task<OperationResult<byte[]>> DownloadVideo(string url, string quality)
    {
        OperationResult<byte[]> result = await _generateTemplateService.DownloadVideo(url, quality);

        return result;
    }

    public OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url)
    {
        OperationResult<List<DownloaderVideoEntity>> result = _generateTemplateService.GetAvailableQualities(url);

        return result;
    }
}
