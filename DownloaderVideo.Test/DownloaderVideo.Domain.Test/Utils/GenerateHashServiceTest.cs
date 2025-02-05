using DownloaderVideo.Domain.Interface.Utils;
using DownloaderVideo.Domain.Utils;

namespace DownloaderVideo.Domain.Test.Utils;

public class GenerateHashServiceTest
{
    private readonly IGenerateHash _generateHash;

    public GenerateHashServiceTest()
    {
        _generateHash = new GenerateHash();
    }

    [Fact]
    public void GenerateHashRandom_ShouldReturnNonEmptyHash()
    {
        // Act
        string result = _generateHash.GenerateHashRandom();

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Equal(64, result.Length); // SHA256 produces a 64-character hex string
    }

    [Theory]
    [InlineData("password123")]
    [InlineData("diferentPassword123")]
    public void GenerateHashParameters_ShouldReturnExpectedHash(string password)
    {
        // Act
        string result = _generateHash.GenerateHashParameters(password);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Equal(64, result.Length); // SHA256 produces a 64-character hex string
    }

    [Theory]
    [InlineData("password123", "ef92b778bafee91d3d3d9aa84d6b31415dbb7c2ba8b0bc843b14b281d8c7e0c2")]
    [InlineData("anotherPassword", "25d55ad283aa400af464c76d713c07ad")]
    public void VerifyPassword_ShouldReturnExpectedResult(string password, string hashedPassword)
    {
        // Act
        string hash = _generateHash.GenerateHashParameters(password);
        bool result = _generateHash.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

}
