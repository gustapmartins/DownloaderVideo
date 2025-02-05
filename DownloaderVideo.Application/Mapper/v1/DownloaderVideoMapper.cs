using AutoMapper;
using DownloaderVideo.Application.Dto.v1;
using DownloaderVideo.Domain.Entity;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Application.Mapper.v1;

[ExcludeFromCodeCoverage]
public class DownloaderVideoMapper : Profile
{
    public DownloaderVideoMapper()
    {
        CreateMap<DownloaderVideoRequest, DownloaderVideoEntity>    ();
        CreateMap<DownloaderVideoEntity, DownloaderVideoResponse>();

        CreateMap(typeof(OperationResult<>), typeof(OperationResult<>));
    }
}
