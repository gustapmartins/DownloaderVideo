namespace DownloaderVideo.Application.Static;

public static class VideoUrlValidator
{
    private static readonly HashSet<string> AllowedDomains = new()
    {
        "youtube.com",
        "www.youtube.com",
        "youtu.be",
        "www.youtu.be"
    };

    public static bool IsValidYouTubeUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return AllowedDomains.Contains(uri.Host);
        }
        return false;
    }
}
