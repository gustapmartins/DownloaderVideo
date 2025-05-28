namespace DownloaderVideo.Domain.Entity;

public class Notification
{
    public Notification(string key, IEnumerable<string> messages)
    {
        Key = key;
        Messages = messages?.ToArray() ?? Array.Empty<string>();
        DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    public string Key { get; }
    public string[] Messages { get; }
    public string DateCreated { get; }
}
