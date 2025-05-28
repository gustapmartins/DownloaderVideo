using System.Text;
using DownloaderVideo.Application.Controllers.v1;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using YoutubeExplode;

public class DownloaderVideoServiceTest
{
    private readonly Mock<INotificationBase> _mockNotification;
    private readonly Mock<YoutubeClient> _mockYoutubeClient;
    private readonly DownloaderVideoService _service;

    public DownloaderVideoServiceTest()
    {
        _mockNotification = new Mock<INotificationBase>();
        _mockYoutubeClient = new Mock<YoutubeClient>();

        _service = new DownloaderVideoService(
            _mockNotification.Object,
            _mockYoutubeClient.Object
        );
    }
}
