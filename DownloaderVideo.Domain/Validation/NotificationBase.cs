using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using System.Collections.Concurrent;

namespace DownloaderVideo.Domain.Validation;

public class NotificationBase : INotificationBase
{
    private readonly ConcurrentBag<Notification> _notifications = new();

    public IEnumerable<Notification> GetNotifications()
    {
        return _notifications.ToList(); // Retorna uma cópia como lista
    }

    public bool HasNotifications()
    {
        return _notifications.Any();
    }

    public Task NotifyAsync(string key, string message)
    {
        _notifications.Add(new Notification(key, new string[1] { message }));
        return Task.CompletedTask;
    }

    public Task NotifyAsync(string key, string[] messages)
    {
        _notifications.Add(new Notification(key, messages));
        return Task.CompletedTask;
    }

    public Task NotifyAsync(string key, List<string> messages)
    {
        _notifications.Add(new Notification(key, messages.ToArray()));
        return Task.CompletedTask;
    }
}
