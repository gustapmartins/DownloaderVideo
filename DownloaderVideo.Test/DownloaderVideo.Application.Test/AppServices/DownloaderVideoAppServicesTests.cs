using AutoFixture;
using AutoMapper;
using DownloaderVideo.Application.AppServices.v1;
using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.Extensions.Logging;
using Moq;

namespace DownloaderVideo.Application.Test.AppServices;

public class DownloaderVideoAppServicesTests
{
    private readonly Mock<IDownloaderVideoService> _mockService;
    private readonly Mock<ILogger<DownloaderVideoAppServices>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DownloaderVideoAppServices _appService;
    private readonly Fixture _fixture;

    public DownloaderVideoAppServicesTests()
    {
        _mockService = new Mock<IDownloaderVideoService>();
        _mockLogger = new Mock<ILogger<DownloaderVideoAppServices>>();
        _mockMapper = new Mock<IMapper>();
        _fixture = new Fixture();

        _appService = new DownloaderVideoAppServices(
            _mockLogger.Object,
            _mockService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public void DownloadVideo_ShouldReturnStream_WhenServiceReturnsResult()
    {
        // Arrange
        var expectedStream = new MemoryStream(new byte[] { 1, 2, 3, 4 }); // cria stream manualmente
        var expectedResult = new OperationResult<Stream>(
            expectedStream,
            "Success",
            200,
            true
        );

        _mockService
            .Setup(s => s.DownloadVideo(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedResult);

        // Act
        var result = _appService.DownloadVideo("http://example.com", "720p");

        // Assert
        Assert.True(result.Status);
        Assert.Equal(expectedStream, result.Content);
    }

    [Fact]
    public async Task GetAvailableQualitiesAsync_ShouldReturnList_WhenServiceReturnsResultAsync()
    {
        // Arrange
        var expectedList = _fixture.CreateMany<DownloaderVideoEntity>(2).ToList();

        var expectedResult = new OperationResult<List<DownloaderVideoEntity>>(
            expectedList,
            "Success",
            200,
            true
        );

        _mockService
            .Setup(s => s.GetAvailableQualitiesAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _appService.GetAvailableQualitiesAsync("http://example.com");

        // Assert
        Assert.True(result.Status);
        Assert.Equal(2, result.Content.Count);
        //Assert.Contains(result.Content, q => q.Resolution == "720p");
    }
} 