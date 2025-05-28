using DownloaderVideo.Domain.Exceptions;
using DownloaderVideo.Domain.Interface.Services;
using Microsoft.Extensions.Caching.Memory;
using VarzeaLeague.Domain.Service;
using Moq;

namespace DownloaderVideo.Domain.Test.Services.v1;

public class MemoryCacheServiceTest
{
    private readonly Mock<IMemoryCache> _memoryCache;
    private readonly IMemoryCacheService _memoryCacheService;

    public MemoryCacheServiceTest()
    {
        _memoryCache = new Mock<IMemoryCache>();
        _memoryCacheService = new MemoryCacheService(_memoryCache.Object);
    }

    [Fact]
    public void AddToCache_ShouldAddItemToCache()
    {
        // Arrange
        string cacheKey = "testKey";
        string cacheValue = "testValue";
        int cacheTime = 5;

        var cacheEntryMock = new Mock<ICacheEntry>();

        _memoryCache
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        // Act
        _memoryCacheService.AddToCache(cacheKey, cacheValue, cacheTime);

        // Assert
        _memoryCache.Verify(m => m.CreateEntry(cacheKey), Times.Once);
    }

    [Fact]
    public void GetCache_ShouldReturnItemFromCache()
    {
        // Arrange
        string cacheKey = "testKey";
        string expectedValue = "testValue";
        object expectedValueObj = expectedValue;

        _memoryCache.Setup(m => m.TryGetValue(cacheKey, out expectedValueObj)).Returns(true);

        // Act
        var result = _memoryCacheService.GetCache<string>(cacheKey);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetCache_ThrowsExceptionCache()
    {
        // Arrange
        string expectedValue = "testValue";
        object expectedValueObj = expectedValue;

        _memoryCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out expectedValueObj)).Throws(new ExceptionFilter("Simulated exception"));

        //Act
        var exception = Assert.Throws<ExceptionFilter>(() => _memoryCacheService.GetCache<string>(It.IsAny<string>()));

        //Assert
        Assert.Equal("Simulated exception", exception.Message);
    }

    [Fact]
    public void GetCache_ShouldReturnDefault_WhenItemNotInCache()
    {
        // Arrange
        string cacheKey = "testKey";
        object expectedValue = null;

        _memoryCache.Setup(m => m.TryGetValue(cacheKey, out expectedValue)).Returns(false);

        // Act
        var result = _memoryCacheService.GetCache<string>(cacheKey);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RemoveFromCache_ShouldRemoveItemFromCache()
    {
        // Arrange
        string cacheKey = "testKey";

        // Act
        _memoryCacheService.RemoveFromCache<string>(cacheKey);

        // Assert
        _memoryCache.Verify(m => m.Remove(cacheKey), Times.Once);
    }

    [Fact]
    public void GetOrCreate_ShouldReturnItemFromCache_IfExists()
    {
        // Arrange
        string cacheKey = "testKey";
        string expectedValue = "testValue";
        object expectedValueObj = expectedValue;
        _memoryCache.Setup(m => m.TryGetValue(cacheKey, out expectedValueObj)).Returns(true);

        // Act
        var result = _memoryCacheService.GetOrCreate(cacheKey, () => "newValue", 5);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetOrCreate_ShouldCreateAndReturnNewItem_IfNotInCache()
    {
        // Arrange
        string expectedValue = "newValue";
        object cacheValue = null;
        _memoryCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out cacheValue)).Returns(false);

        var cacheEntryMock = new Mock<ICacheEntry>();
        _memoryCache
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        // Act
        var result = _memoryCacheService.GetOrCreate(It.IsAny<string>(), () => expectedValue, 5);

        // Assert
        Assert.Equal(expectedValue, result);
        cacheEntryMock.VerifySet(ce => ce.Value = expectedValue, Times.Once);
        _memoryCache.Verify(m => m.CreateEntry(It.IsAny<string>()), Times.Once);
    }
}
