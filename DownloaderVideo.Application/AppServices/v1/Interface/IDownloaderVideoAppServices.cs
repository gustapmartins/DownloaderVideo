﻿using DownloaderVideo.Domain.Entity;

namespace DownloaderVideo.Application.AppServices.v1.Interfaces;

public interface IDownloaderVideoAppServices
{
    OperationResult<Stream> DownloadVideo(string url, string quality);
    Task<OperationResult<List<DownloaderVideoEntity>>> GetAvailableQualitiesAsync(string url);
}
