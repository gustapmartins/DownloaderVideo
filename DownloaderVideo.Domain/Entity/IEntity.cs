namespace DownloaderVideo.Domain.Entity;

public interface IEntity<TId>
{
    public TId Id { get; set; }

    public DateTime DateCreated { get; set; }
}
